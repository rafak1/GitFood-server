namespace Server.Logic.Abstract.Authentication;

public interface IPasswordChecker
{
    bool IsCorrectPassword(string password);
    bool isCorrectLogin(string login);
}