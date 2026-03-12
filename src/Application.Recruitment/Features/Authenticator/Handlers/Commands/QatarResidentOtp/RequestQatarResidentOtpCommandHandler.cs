using System.Security.Cryptography;
using Application.Recruitment.Common.Interfaces.Services;
using Application.Recruitment.Common.Interfaces.Services.HttpClients;
using Application.Recruitment.Features.Authenticator.Commands.QatarLogin;
using Application.Recruitment.Features.Authenticator.Handlers.Utils;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services.Notifications;
using Tawtheef.Application.Common.Utils;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.TestData;

namespace Application.Recruitment.Features.Authenticator.Handlers.Commands.QatarResidentOtp
{
    public sealed class RequestQatarResidentOtpCommandHandler(
        IQatarResidentVerificationClient verificationClient,
        UserManager<User> userManager, IMoiService moiService,
        ISmsSender smsSender, TimeProvider timeProvider, IAppLogger logger
    ) : IRequestHandler<RequestQatarResidentOtpCommand, IResult<Unit>>
    {
        private readonly IAppLogger _log = logger.ForContext(typeof(RequestQatarResidentOtpCommandHandler));
        public async Task<IResult<Unit>> Handle(RequestQatarResidentOtpCommand request, CancellationToken cancellationToken)
        {
            var normalizedQid = QidUtilities.Normalize(request.Qid);
            var qidMasked = MoiUtils.MaskQid(normalizedQid);

            _log.Information("Request Qatar resident OTP started. Qid={QidMasked}", qidMasked);

            if (!QidUtilities.IsValid(normalizedQid))
            {
                _log.Warning("Invalid QID. Qid={QidMasked}", qidMasked);
                return Result.Fail<Unit>(ErrorsCodes.QatarResidentInvalidQid);
            }

            if (!MoiUtils.IsQatarMobileNumber(request.PhoneNumber))
            {
                _log.Warning("Invalid Qatar mobile format (must start with +974). Qid={QidMasked}", qidMasked);
                return Result.Fail<Unit>(ErrorsCodes.QatarResidentPhoneInvalid);
            }

            var normalizedPhone = MoiUtils.NormalizePhone(request.PhoneNumber);
            if (string.IsNullOrWhiteSpace(normalizedPhone))
            {
                _log.Warning("Phone required/invalid after normalization. Qid={QidMasked}", qidMasked);
                return Result.Fail<Unit>(ErrorsCodes.UserPhoneRequired);
            }

            if(TestData.QID_TEST().Contains(long.Parse(normalizedQid)))
            {
                _log.Information("Test QID detected, skipping verification. Qid={QidMasked}", qidMasked);
            }
            else
            {
                var verification = await verificationClient.VerifyAsync(normalizedQid, normalizedPhone, cancellationToken);
                if (verification.IsFailed)
                {
                    _log.Warning(
                        "Qatar resident verification failed. Qid={QidMasked} Errors={Errors}",
                        qidMasked,
                        string.Join(" | ", verification.Errors.Select(e => e.Message)));

                    return Result.Fail<Unit>(verification.Errors);
                }
            }

            if (!string.Equals(QidUtilities.Normalize(normalizedQid), normalizedQid, StringComparison.Ordinal))
            {
                _log.Warning("QID normalization mismatch detected. Qid={QidMasked}", qidMasked);
                return Result.Fail<Unit>(ErrorsCodes.QatarResidentInvalidQid);
            }

            var user = await userManager.FindByLoginAsync(QatarResidentOtpConstants.Provider, normalizedQid);
            var placeholderEmail = $"qr{normalizedQid}{QatarResidentOtpConstants.PlaceholderEmailDomain}";

            if (user is null)
            {
                _log.Information("User not found by login; trying placeholder email. Qid={QidMasked}", qidMasked);

                user = await userManager.FindByEmailAsync(placeholderEmail);
                if (user is null)
                {
                    _log.Information("Creating placeholder user for OTP flow. Qid={QidMasked}", qidMasked);

                    var newUser = User.Register(
                        placeholderEmail,
                        QatarResidentOtpConstants.DisplayName,
                        UserTypeIds.Applicant);

                    if (newUser.IsFailed)
                    {
                        _log.Warning(
                            "User.Register failed for placeholder user. Qid={QidMasked} Errors={Errors}",
                            qidMasked,
                            string.Join(" | ", newUser.Errors.Select(e => e.Message)));

                        return Result.Fail<Unit>(newUser.Errors);
                    }

                    user = newUser.Value;
                    user.PhoneNumber = normalizedPhone;
                    user.PhoneNumberConfirmed = false;

                    var createRes = await userManager.CreateAsync(user);
                    if (!createRes.Succeeded)
                    {
                        _log.Error(
                            "CreateAsync failed for placeholder user. Qid={QidMasked} Errors={Errors}",
                            qidMasked,
                            string.Join(", ", createRes.Errors.Select(e => e.Description)));

                        return FailureFromIdentity<Unit>(createRes);
                    }
                }

                var addLogin = await userManager.AddLoginAsync(
                    user,
                    new UserLoginInfo(
                        QatarResidentOtpConstants.Provider,
                        normalizedQid,
                        QatarResidentOtpConstants.Provider));

                if (!addLogin.Succeeded)
                {
                    _log.Error(
                        "AddLoginAsync failed. UserId={UserId} Qid={QidMasked} Errors={Errors}",
                        user.Id,
                        qidMasked,
                        string.Join(", ", addLogin.Errors.Select(e => e.Description)));

                    return FailureFromIdentity<Unit>(addLogin);
                }
            }
            else
            {
                _log.Information("User found by login. UserId={UserId} Qid={QidMasked}", user.Id, qidMasked);
                user.PhoneNumber = normalizedPhone;
                user.PhoneNumberConfirmed = false;
            }

            var personalInfoResult = await moiService.GetMoiPersonalInfoWithKawaderCheckAsync(normalizedQid, request.QidExpiry,
                cancellationToken);
            if (personalInfoResult.IsFailed)
            {
                _log.Warning(
                    "Login blocked by Kawader check. UserId={UserId} Qid={QidMasked} Errors={Errors}",
                    user.Id,
                    qidMasked,
                    string.Join(" | ", personalInfoResult.Errors.Select(e => e.Message)));

                return Result.Fail<Unit>(personalInfoResult.Errors);
            }
        
            var now = timeProvider.GetUtcNow().UtcDateTime;

            var canSend = user.CanSendOtp(now, QatarResidentOtpConstants.MaxOtpSends, 
                QatarResidentOtpConstants.OtpSendWindow);

            if (canSend.IsFailed)
            {
                _log.Warning(
                    "OTP send not allowed (rate/lock rules). UserId={UserId} Qid={QidMasked} Errors={Errors}",
                    user.Id, qidMasked, string.Join(" | ", canSend.Errors.Select(e => e.Message)));

                return Result.Fail<Unit>(canSend.Errors);
            }

            if(TestData.QID_TEST().Contains(long.Parse(normalizedQid)))
            {
                _log.Information("Test QID detected, skipping OTP sending. Qid={QidMasked}", qidMasked);
                user.SetOtpReference("123456", now.AddMinutes(QatarResidentOtpConstants.OtpExpiryMinutes));
                var update = await userManager.UpdateAsync(user);
                if (!update.Succeeded)
                {
                    _log.Error(
                        "UpdateAsync failed after setting OTP reference. UserId={UserId} Errors={Errors}",
                        user.Id, string.Join(", ", update.Errors.Select(e => e.Description)));
                }
                user.MarkOtpSent();
                update = await userManager.UpdateAsync(user);
                if (!update.Succeeded)
                {
                    _log.Error(
                        "UpdateAsync failed after MarkOtpSent. UserId={UserId} Errors={Errors}",
                        user.Id, string.Join(", ", update.Errors.Select(e => e.Description)));

                    return FailureFromIdentity<Unit>(update);
                }
            }
            else
            {
                var otp = GenerateCode(QatarResidentOtpConstants.OtpLength); // DO NOT LOG THIS
                user.SetOtpReference(otp, now.AddMinutes(QatarResidentOtpConstants.OtpExpiryMinutes));

                var update = await userManager.UpdateAsync(user);
                if (!update.Succeeded)
                {
                    _log.Error(
                        "UpdateAsync failed after setting OTP reference. UserId={UserId} Errors={Errors}",
                        user.Id, string.Join(", ", update.Errors.Select(e => e.Description)));

                    return FailureFromIdentity<Unit>(update);
                }

                // DO NOT log OTP content. (Even in dev.)
                _ = await smsSender.SendAsync(normalizedPhone, $"Your verification code is: {otp}", cancellationToken);

#if DEBUG
                Console.WriteLine($"[DEBUG] OTP for UserId={user.Id} Qid={qidMasked}: {otp}");
#endif

                user.MarkOtpSent();

                update = await userManager.UpdateAsync(user);
                if (!update.Succeeded)
                {
                    _log.Error(
                        "UpdateAsync failed after MarkOtpSent. UserId={UserId} Errors={Errors}",
                        user.Id, string.Join(", ", update.Errors.Select(e => e.Description)));

                    return FailureFromIdentity<Unit>(update);
                }
            }

            _log.Information("Request Qatar resident OTP succeeded. UserId={UserId} Qid={QidMasked}", user.Id, qidMasked);

            return Result.Ok(Unit.Value);
        }

        private static string GenerateCode(int length)
        {
            const string digits = "0123456789";
            var bytes = RandomNumberGenerator.GetBytes(length);
            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = digits[bytes[i] % digits.Length];

            return new string(chars);
        }

        private static Result<T> FailureFromIdentity<T>(IdentityResult res) =>
            Result.Fail<T>(string.Join(", ", res.Errors.Select(e => e.Description)));
    }
}

