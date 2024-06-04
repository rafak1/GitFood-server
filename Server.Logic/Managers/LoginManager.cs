using Microsoft.EntityFrameworkCore;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Authentication;
using Server.Logic.Abstract.Token;
using Server.Data.Models;
using Server.Database;
using Server.Logic.Abstract;
using Server.ViewModels.Login;
using Server.Logic.Abstract.Email;
using Server.Logic.Users;

namespace Server.Logic.Managers;

internal class LoginManager : ILoginManager
{
    private readonly GitfoodContext _dbInfo;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IPasswordChecker _checker;
    private readonly ITokenStorage _tokenStorage;
    private readonly IEmailManager _emailManager;
    private static Random generator = new Random();
    
    private const string _InvalidLoginOrPasswordMessage = "Invalid login or password";
    private const string _NotVerifiedMessage = "User is not verified";
    private const string _InvalidEmailMessage = "Invalid email";
    private const string _LoginTakenMessage = "Login is already taken";
    private const string _EmailTakenMessage = "Email is already taken";
    private const string _BannedEmailMessage = "Email is banned, be ashamed of yourself!";
    private const string _BadVerificationTokenMessage = "Invalid verification token";

    public LoginManager(GitfoodContext database, ITokenGenerator tokenGenerator, IPasswordChecker checker, ITokenStorage tokenStorage, IEmailManager emailManager)
    {
        _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
        _checker = checker ?? throw new ArgumentNullException(nameof(checker));
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
        _emailManager = emailManager ?? throw new ArgumentNullException(nameof(emailManager));
    }

    public async Task<IManagerActionResult<string>> LoginAsync(LoginViewModel login)
    {
        var isCorrect = await _dbInfo.Users.FirstOrDefaultAsync(
            x => x.Login == login.Login && x.Password == login.Password);

        if(isCorrect == null) 
            return new ManagerActionResult<string>(null, ResultEnum.BadRequest, _InvalidLoginOrPasswordMessage);
        else if(isCorrect.Verification != null)
            return new ManagerActionResult<string>(null, ResultEnum.BadRequest, _NotVerifiedMessage);
        else if(isCorrect.IsBanned)
            return new ManagerActionResult<string>(null, ResultEnum.BadRequest, _BannedEmailMessage);
        else
        {
            var token = _tokenGenerator.GrantToken();
            _tokenStorage.AddToken(token, login.Login);
            return new ManagerActionResult<string>(token, ResultEnum.OK);
        }
    }

    public async Task<IManagerActionResult> RegisterAsync(RegisterViewModel login) 
        => await new DatabaseExceptionHandler().HandleExceptionsAsync(async () => await RegisterInternalAsync(login));

    private async Task<IManagerActionResult> RegisterInternalAsync(RegisterViewModel login)
    {
        if(!_checker.IsCorrectPassword(login.Password) || !_checker.isCorrectLogin(login.Login))
            return new ManagerActionResult(ResultEnum.BadRequest, _InvalidLoginOrPasswordMessage);

        if(!_emailManager.isValid(login.Email))
            return new ManagerActionResult(ResultEnum.BadRequest, _InvalidEmailMessage);

        if(_emailManager.isBanned(login.Email))
            return new ManagerActionResult(ResultEnum.BadRequest, _BannedEmailMessage);

        if(isTaken(login.Login))
            return new ManagerActionResult(ResultEnum.BadRequest, _LoginTakenMessage);

        if(isEmailTaken(login.Email))
            return new ManagerActionResult(ResultEnum.BadRequest, _EmailTakenMessage);

        String verificationToken = generateVerificationToken();

        _emailManager.sendVerificationEmail(login.Email, verificationToken, login.Login);

        await _dbInfo.Users.AddAsync(new User
        {
            Login = login.Login,
            Password = login.Password,
            Email = login.Email,
            Verification = verificationToken
        });
        await _dbInfo.SaveChangesAsync();
        
        return new ManagerActionResult(ResultEnum.OK);
    }

    private bool isTaken(string login) => _dbInfo.Users.Any(x => x.Login == login);
    private bool isEmailTaken(string email) => _dbInfo.Users.Any(x => x.Email == email);
    private string generateVerificationToken()
    {
        int r = generator.Next(1, 1000000);
        return r.ToString().PadLeft(6, '0');
    }

    public Task<IManagerActionResult> VerifyAsync(string token, string login)
    {
        var user = _dbInfo.Users.FirstOrDefault(x => x.Login == login && x.Verification == token);
        if(user == null)
            return Task.FromResult<IManagerActionResult>(new ManagerActionResult(ResultEnum.BadRequest, _BadVerificationTokenMessage));
        else
        {
            user.Verification = null;
            _dbInfo.SaveChanges();
            return Task.FromResult<IManagerActionResult>(new ManagerActionResult(ResultEnum.OK));
        }
    }

    public async Task<IManagerActionResult> BanAsync(string toBan, string user)
    {
        if (Enum.IsDefined(typeof(ElevatedUsers), user))
        {
            _dbInfo.Users.FirstOrDefault(x => x.Login == toBan).IsBanned = true;
            await _dbInfo.SaveChangesAsync();
            return new ManagerActionResult(ResultEnum.OK);
        }
        else
        {
            return new ManagerActionResult(ResultEnum.Unauthorizated);
        }
    }
}