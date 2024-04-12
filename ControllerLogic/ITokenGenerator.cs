public interface ITokenGenerator
{
    string GrantToken(string JwtKey, string JwtIssuer);
}