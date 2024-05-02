namespace Server.Logic.Abstract.Token;

public interface ITokenStorage
{
    void AddToken(string token, string user);
    string GetUser(string token);
    void RemoveUser(string user);
}