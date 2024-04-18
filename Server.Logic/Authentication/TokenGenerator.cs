using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Server.Logic.Abstract.Authentication;

namespace Server.Logic.Authentication;

public class TokenGenerator : ITokenGenerator
{
    private readonly ITokenConfigProvider _tokenConfigProvider;

    public TokenGenerator(ITokenConfigProvider tokenConfigProvider)
    {
        _tokenConfigProvider = tokenConfigProvider ?? throw new ArgumentNullException(nameof(tokenConfigProvider));
    }

    public string GrantToken(){
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfigProvider.GetJwtKey()));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var Sectoken = new JwtSecurityToken(_tokenConfigProvider.GetJwtIssuer(),
            _tokenConfigProvider.GetJwtIssuer(),
            null,
            expires: DateTime.Now.AddMinutes(_tokenConfigProvider.GetJwtExpireMinutes()),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(Sectoken);
    }
}