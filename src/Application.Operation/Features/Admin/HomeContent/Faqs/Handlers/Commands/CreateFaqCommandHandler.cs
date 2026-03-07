using Application.Operation.Features.Admin.HomeContent.Faqs.Commands;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Content;

namespace Application.Operation.Features.Admin.HomeContent.Faqs.Handlers.Commands;

public sealed class CreateFaqCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateFaqCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(
        CreateFaqCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<FAQ>();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);
        var now = timeProvider.GetUtcNow();

        var newFaq = new FAQ
        {
            QuestionAr = request.QuestionAr.Trim(),
            QuestionEn = request.QuestionEn.Trim(),
            AnswerAr = request.AnswerAr.Trim(),
            AnswerEn = request.AnswerEn.Trim(),
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            CreatedDate = now.UtcDateTime,
            CreatedById = hasUser ? userId : null
        };

        await repository.AddAsync(newFaq);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(newFaq.Id);
    }
}

