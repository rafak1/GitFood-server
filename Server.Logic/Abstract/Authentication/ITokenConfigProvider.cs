namespace Server.Logic.Abstract.Authentication;

public interface ITokenConfigProvider
{
    string GetJwtKey();
    string GetJwtIssuer();
    int GetJwtExpireMinutes();
    int GetJwtPurgeInterval();
}