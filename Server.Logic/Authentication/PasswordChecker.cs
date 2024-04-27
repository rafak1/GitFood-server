using Server.Logic.Abstract.Authentication;

namespace Server.Logic.Authentication;

internal class PasswordChecker : IPasswordChecker
{
    private const int _minPasswordLength = 5;
    private const int _minLoginLength = 5;

    public bool IsCorrectPassword(string password) 
        => password.Length > _minPasswordLength &&
            password.Any(char.IsDigit) &&
            !password.Any(char.IsWhiteSpace);

    public bool isCorrectLogin(string login) 
        => login.Length > _minLoginLength && !login.Any(char.IsWhiteSpace);
    
}
