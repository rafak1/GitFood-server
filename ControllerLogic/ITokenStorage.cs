public interface ITokenStorage
{
    void AddToken(string token, string user);
    string getUser(string token);
}