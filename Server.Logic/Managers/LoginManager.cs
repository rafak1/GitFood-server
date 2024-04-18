using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Authentication;
using Server.Logic.Abstract.Token;
using Server.Data.Models;
using Server.Database;

namespace Server.Logic.Managers;

internal class LoginManager : ILoginManager
{
    private readonly GitfoodContext _dbInfo;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IPasswordChecker _checker;
    private readonly ITokenStorage _tokenStorage;
    private const string _InvalidLoginOrPasswordMessage = "Invalid login or password";

    public LoginManager(GitfoodContext database, ITokenGenerator tokenGenerator, IPasswordChecker checker, ITokenStorage tokenStorage)
    {
        _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
        _checker = checker ?? throw new ArgumentNullException(nameof(checker));
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
    }

    public async Task<string> LoginAsync(LoginRequest login)
    {
        var isCorrect = await _dbInfo.Users.FirstOrDefaultAsync(
            x => x.Login == login.Email && x.Password == login.Password);

        if(isCorrect == null) 
            return _InvalidLoginOrPasswordMessage; // BadRequest
        else
        {
            var token = _tokenGenerator.GrantToken();
            _tokenStorage.AddToken(token, login.Email);
            // OK
            return token;
        }
    }

    public async Task<string> RegisterAsync(LoginRequest login) 
    {
        if(!_checker.IsCorrectPassword(login.Password) || !_checker.isCorrectLogin(login.Email))
            return _InvalidLoginOrPasswordMessage; //BadRequest

        await _dbInfo.Users.AddAsync(new User
        {
            Login = login.Email,
            Password = login.Password
        });
        await _dbInfo.SaveChangesAsync();

        //Error handling?
        
        var token = _tokenGenerator.GrantToken();
        _tokenStorage.AddToken(token, login.Email);
        return token; // OK
    }
}