using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetLanguageLevelsQueryHandler(
    IUnitOfWork unitOfWork)
    : BaseLookupQueryHandler<LanguageLevel, GetLanguageLevelsQuery>(unitOfWork);