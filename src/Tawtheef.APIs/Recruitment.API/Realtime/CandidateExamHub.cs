using System.Security.Claims;
using Application.Recruitment.Features.CandidateExams.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Recruitment.API.Realtime;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class CandidateExamHub(
    CandidateExamConnectionService connectionService,
    CandidateExamConnectionRegistry connectionRegistry,
    CandidateExamResumeService resumeService,
    ILogger<CandidateExamHub> logger) : Hub
{
    public async Task RegisterAttempt(
        Guid testAttemptId,
        Guid activeSessionId)
    {
        if (!Guid.TryParse(
                Context.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
            throw Unavailable();

        try
        {
            var registered = await connectionService.RegisterAsync(
                userId,
                testAttemptId,
                activeSessionId,
                Context.ConnectionId,
                Context.ConnectionAborted);

            if (!registered)
                throw Unavailable();

            connectionRegistry.Set(Context.ConnectionId, testAttemptId);
        }
        catch (HubException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Candidate exam connection registration failed for attempt {AttemptId}",
                testAttemptId);

            throw Unavailable();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        connectionRegistry.TryRemove(Context.ConnectionId, out _);

        try
        {
            await connectionService.DisconnectAsync(Context.ConnectionId, CancellationToken.None);
        }
        catch (Exception cleanupException)
        {
            logger.LogError(cleanupException,
                "Candidate exam connection cleanup failed for connection {ConnectionId}",
                Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task<CandidateExamResumeResult> ResumeAttempt(Guid testAttemptId,string accessCode)
    {
        if (!Guid.TryParse(Context.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            throw Unavailable();

        try
        {
            var result = await resumeService.ResumeAsync(userId, testAttemptId,accessCode, Context.ConnectionAborted);
            return result ?? throw Unavailable();
        }
        catch (HubException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception,
                "Candidate exam resume failed for attempt {AttemptId}", testAttemptId);
            throw Unavailable();
        }
    }

    private static HubException Unavailable() =>
        new("Candidate exam registration is not available.");
}
