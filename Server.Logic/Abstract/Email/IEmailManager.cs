namespace Server.Logic.Abstract.Email;

public interface IEmailManager
{
    bool isValid(string email);
    bool isBanned(string email);
    bool isVerified(string email);
    void sendVerificationEmail(string email, string verificationToken, string user);
}