public interface ITokenStorage
{
    void AddToken(string token, string user);
    string GetUser(string token);
}