using Microsoft.Extensions.Configuration;
using Server.Logic.Abstract.Authentication;

namespace Server.Logic.Authentication;

internal class TokenConfigProvider : ITokenConfigProvider
{
    private readonly IConfiguration _config;

    public TokenConfigProvider(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public int GetJwtPurgeInterval() => int.Parse(_config["Jwt:PurgeInterval"]);

    public int GetJwtExpireMinutes() => int.Parse(_config["Jwt:ExpireMinutes"]);

    public string GetJwtIssuer() => _config["Jwt:Issuer"];

    public string GetJwtKey() => _config["Jwt:Key"];
}