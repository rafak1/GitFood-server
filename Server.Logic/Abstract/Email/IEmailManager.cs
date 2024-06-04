namespace Server.Logic.Abstract.Email;

public interface IEmailManager
{
    bool isValid(string email);
    Task<bool> isBannedAsync(string email);
    Task<bool> isVerifiedAsync(string email);
    void sendVerificationEmailAsync(string email, string verificationToken, string user);
}