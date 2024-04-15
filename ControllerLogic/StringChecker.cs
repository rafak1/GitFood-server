public class StringChecker : IStringChecker
{
    public Boolean IsCorrectPassword(String password)
    {
        return password.Length > 5 &&
                password.Any(char.IsDigit) &&
               !password.Any(char.IsWhiteSpace);
    }

    public Boolean isCorrectLogin(String login)
    {
        return login.Length > 5 &&
                !login.Any(char.IsWhiteSpace);
    }
}