public class TokenConfigProvider : ITokenConfigProvider
{
    private readonly IConfiguration _config;

    public TokenConfigProvider(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public int GetJwtPurgeInterval()
    {
        return int.Parse(_config["Jwt:PurgeInterval"]);
    }

    public int GetJwtExpireMinutes()
    {
        return int.Parse(_config["Jwt:ExpireMinutes"]);
    }

    public string GetJwtIssuer()
    {
        return _config["Jwt:Issuer"];
    }

    public string GetJwtKey()
    {
        return _config["Jwt:Key"];
    }
}