namespace Server.Logic.Abstract.Authentication;

public interface ITokenGenerator
{
    string GrantToken();
}