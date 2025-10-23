using Microsoft.AspNetCore.Identity;

namespace Tawtheef.Application.Features.Authenticator.DTOs;

public record UserSocialAccounts(SocialAccounts Google, SocialAccounts AzureAD);


public record SocialAccounts
{
    public bool Connected { get; set; }
    public string Email { get; set; }
    public string Id { get; set; }
    public string Provider { get; set; }

    public SocialAccounts(UserLoginInfo? loginInfo, string? email)
    {
        Connected = loginInfo is not null;
        Email = email ?? string.Empty;
        Id = loginInfo?.ProviderKey ?? string.Empty;
        Provider = loginInfo?.LoginProvider ?? string.Empty;
    }
}
