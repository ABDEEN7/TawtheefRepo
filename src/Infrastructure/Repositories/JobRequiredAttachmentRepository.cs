using Application.Operation.Common.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

internal class JobRequiredAttachmentRepository(IGenericRepository<JobRequiredAttachment> repository)
    : BaseRepository<JobRequiredAttachment>(repository), IJobRequiredAttachmentRepository;
