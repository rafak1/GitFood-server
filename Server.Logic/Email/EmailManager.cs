using System.ComponentModel.DataAnnotations;
using Server.Database;
using Server.Logic.Abstract.Email;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;

namespace Server.Logic.Email;

internal class EmailManager  : IEmailManager{    
    private readonly GitfoodContext _dbInfo;
    private readonly IConfiguration _configuration;
    private readonly String _email;
    private readonly String _password;

    public EmailManager(GitfoodContext dbInfo, IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _dbInfo = dbInfo ?? throw new ArgumentNullException(nameof(dbInfo));
        _email = _configuration.GetSection("EmailConfig").GetSection("Email").Value;
        _password = _configuration.GetSection("EmailConfig").GetSection("Password").Value;
    }

    public bool isBanned(string email)
    {
        var user = _dbInfo.Users.FirstOrDefault(e => e.Email == email);
        return user != null && user.IsBanned;
    }

    public bool isValid(string email)
    {
        var emailValidator = new EmailAddressAttribute();
        return emailValidator.IsValid(email);
    }

    public bool isVerified(string email)
    {
        var user = _dbInfo.Users.FirstOrDefault(e => e.Email == email);
        return user != null && user.Verification == null;
    }

    public void sendVerificationEmail(string email, string verificationToken, string user)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(name: "gitfood.fun", address: _email));
        message.To.Add(new MailboxAddress(name: "", address: email));
        message.Subject = "Weryfikacja konta Git Food";

        message.Body = new TextPart("plain")
        {
            Text = "Witaj " + user + " !\n" +
                "Cieszymy się, że dołączyłeś do naszej społeczności. Kod weryfikacyjny to: " + verificationToken + "\n"
        };

        using var client = new SmtpClient();
        client.Connect(host: "smtp.gmail.com", port: 465, useSsl: true);
        client.Authenticate(userName: _email, password: _password);
        client.Send(message);
        client.Disconnect(quit: true);
    }
}