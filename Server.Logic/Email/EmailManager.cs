using System.ComponentModel.DataAnnotations;
using Server.Database;
using Server.Logic.Abstract.Email;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Server.Logic.Email;

internal class EmailManager  : IEmailManager{    
    private readonly GitfoodContext _dbInfo;
    private readonly IConfiguration _configuration;
    private readonly string _email;
    private readonly string _password;
    private readonly string _stmpServer;

    private const string _subject = "Git Food account verification";
    private const int _sslPort = 465;
    private const string _verificationMessage = "Hello, {0}! \n Welcome to Git Food! \n Your verification code is: {1}";

    public EmailManager(GitfoodContext dbInfo, IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _dbInfo = dbInfo ?? throw new ArgumentNullException(nameof(dbInfo));
        _email = _configuration.GetSection("EmailConfig").GetSection("Email").Value;
        _password = _configuration.GetSection("EmailConfig").GetSection("Password").Value;
        _stmpServer = _configuration.GetSection("EmailConfig").GetSection("StmpServer").Value;
    }

    public async Task<bool> isBannedAsync(string email)
        => await _dbInfo.Users.Where(e => e.Email == email).AnyAsync(x => x.IsBanned);

    public bool isValid(string email)
    {
        var emailValidator = new EmailAddressAttribute();
        return emailValidator.IsValid(email);
    }

    public async Task<bool> isVerifiedAsync(string email)
        => await _dbInfo.Users.Where(e => e.Email == email).AnyAsync(x => x.Verification == null);


    public async void sendVerificationEmailAsync(string email, string verificationToken, string user)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(name: _email, address: _email));
        message.To.Add(new MailboxAddress(name: "", address: email));
        message.Subject = _subject;

        message.Body = new TextPart("plain")
        {
            Text = String.Format(_verificationMessage, user, verificationToken)
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(host: _stmpServer, port: _sslPort, useSsl: true);
        await client.AuthenticateAsync(userName: _email, password: _password);
        await client.SendAsync(message);
        await client.DisconnectAsync(quit: true);
    }
}