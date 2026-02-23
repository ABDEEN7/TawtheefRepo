using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Infrastructure.Data;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JobTitlesController(TawtheefDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> List()
    {
        var data = await dbContext.JobTitle
            .AsNoTracking()
            .OrderBy(x => x.JobNameEn)
            .ToListAsync();

        return Ok(data);
    }

    [HttpPost]
    [AuthorizePermission(PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> Create([FromBody] UpsertJobTitleRequest request)
    {
        if (await dbContext.JobTitle.AnyAsync(x => x.JobNumber == request.JobNumber && !x.IsDeleted))
            return BadRequest("JOB_NUMBER_ALREADY_EXISTS");

        var entity = new JobTitle
        {
            Id = Guid.NewGuid(),
            JobNumber = request.JobNumber.Trim(),
            JobNameAr = request.JobNameAr.Trim(),
            JobNameEn = request.JobNameEn.Trim(),
            IsActive = true
        };

        dbContext.JobTitle.Add(entity);
        await dbContext.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id:guid}")]
    [AuthorizePermission(PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertJobTitleRequest request)
    {
        var entity = await dbContext.JobTitle.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return NotFound();

        if (await dbContext.JobTitle.AnyAsync(x => x.Id != id && x.JobNumber == request.JobNumber && !x.IsDeleted))
            return BadRequest("JOB_NUMBER_ALREADY_EXISTS");

        entity.JobNumber = request.JobNumber.Trim();
        entity.JobNameAr = request.JobNameAr.Trim();
        entity.JobNameEn = request.JobNameEn.Trim();

        await dbContext.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpDelete("{id:guid}")]
    [AuthorizePermission(PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await dbContext.JobTitle.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return NotFound();

        var isUsed = await dbContext.Jobs.AnyAsync(x => x.JobTitleId == id && !x.IsDeleted);
        if (isUsed)
            return BadRequest("JOB_TITLE_IN_USE");

        dbContext.JobTitle.Remove(entity);
        await dbContext.SaveChangesAsync();
        return Ok(true);
    }

    public sealed class UpsertJobTitleRequest
    {
        public required string JobNumber { get; set; }
        public required string JobNameAr { get; set; }
        public required string JobNameEn { get; set; }
    }
}
