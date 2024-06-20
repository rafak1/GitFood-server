namespace Server.Logic.Abstract.Email;

public interface IEmailManager
{
    bool IsValid(string email);
    Task<bool> IsBannedAsync(string email);
    Task<bool> IsVerifiedAsync(string email);
    Task SendVerificationEmailAsync(string email, string verificationToken, string user);
}