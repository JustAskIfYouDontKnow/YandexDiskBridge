using YandexDiskBridge.API.Services.Base;

namespace YandexDiskBridge.API.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string? GetOAuthToken()
    {
        return _configuration["OAuthToken"];
    }
}