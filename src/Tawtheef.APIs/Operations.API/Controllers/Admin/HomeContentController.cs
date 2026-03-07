using Application.Operation.Features.Admin.HomeContent.Faqs.Commands;
using Application.Operation.Features.Admin.HomeContent.Faqs.Queries;
using Application.Operation.Features.Admin.HomeContent.SuccessStories.Commands;
using Application.Operation.Features.Admin.HomeContent.SuccessStories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Route("api/home-content")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class HomeContentController(IMediator mediator) : ControllerBase
{
    [HttpGet("success-stories")]
    [AuthorizePermission(PermissionKeys.HomeContent.View)]
    public async Task<IActionResult> ListSuccessStories()
    {
        var result = await mediator.Send(new ListHomeSuccessStoriesQuery());
        return result.ToActionResult();
    }

    [HttpGet("success-stories/{id:guid}")]
    [AuthorizePermission(PermissionKeys.HomeContent.View)]
    public async Task<IActionResult> SuccessStoryDetails(Guid id)
    {
        var result = await mediator.Send(new GetHomeSuccessStoryDetailsQuery(id));
        return result.ToActionResult();
    }

    [HttpPost("success-stories")]
    [AuthorizePermission(PermissionKeys.HomeContent.Manage)]
    public async Task<IActionResult> CreateSuccessStory([FromForm] CreateHomeSuccessStoryCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("success-stories/{id:guid}")]
    [AuthorizePermission(PermissionKeys.HomeContent.Manage)]
    public async Task<IActionResult> UpdateSuccessStory(Guid id, [FromForm] UpdateHomeSuccessStoryCommand command)
    {
        var result = await mediator.Send(command with { StoryId = id });
        return result.ToActionResult();
    }

    [HttpPut("success-stories/{id:guid}/status")]
    [AuthorizePermission(PermissionKeys.HomeContent.Manage)]
    public async Task<IActionResult> UpdateSuccessStoryStatus(Guid id, [FromBody] UpdateHomeSuccessStoryStatusCommand command)
    {
        var result = await mediator.Send(command with { StoryId = id });
        return result.ToActionResult();
    }

    [HttpDelete("success-stories/{id:guid}")]
    [AuthorizePermission(PermissionKeys.HomeContent.Manage)]
    public async Task<IActionResult> DeleteSuccessStory(Guid id)
    {
        var result = await mediator.Send(new DeleteHomeSuccessStoryCommand(id));
        return result.ToActionResult();
    }

    [HttpGet("faqs")]
    [AuthorizePermission(PermissionKeys.HomeContent.View)]
    public async Task<IActionResult> ListFaqs()
    {
        var result = await mediator.Send(new ListFaqsQuery());
        return result.ToActionResult();
    }

    [HttpGet("faqs/{id:guid}")]
    [AuthorizePermission(PermissionKeys.HomeContent.View)]
    public async Task<IActionResult> FaqDetails(Guid id)
    {
        var result = await mediator.Send(new GetFaqDetailsQuery(id));
        return result.ToActionResult();
    }

    [HttpPost("faqs")]
    [AuthorizePermission(PermissionKeys.HomeContent.Manage)]
    public async Task<IActionResult> CreateFaq([FromBody] CreateFaqCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("faqs/{id:guid}")]
    [AuthorizePermission(PermissionKeys.HomeContent.Manage)]
    public async Task<IActionResult> UpdateFaq(Guid id, [FromBody] UpdateFaqCommand command)
    {
        var result = await mediator.Send(command with { FaqId = id });
        return result.ToActionResult();
    }

    [HttpPut("faqs/{id:guid}/status")]
    [AuthorizePermission(PermissionKeys.HomeContent.Manage)]
    public async Task<IActionResult> UpdateFaqStatus(Guid id, [FromBody] UpdateFaqStatusCommand command)
    {
        var result = await mediator.Send(command with { FaqId = id });
        return result.ToActionResult();
    }

    [HttpDelete("faqs/{id:guid}")]
    [AuthorizePermission(PermissionKeys.HomeContent.Manage)]
    public async Task<IActionResult> DeleteFaq(Guid id)
    {
        var result = await mediator.Send(new DeleteFaqCommand(id));
        return result.ToActionResult();
    }
}

