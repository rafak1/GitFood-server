public interface ITokenConfigProvider
{
    string GetJwtKey();
    string GetJwtIssuer();
}