using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Authentication;
using Server.Logic.Abstract.Token;
using Server.Data.Models;
using Server.Database;
using Server.Logic.Abstract;

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

    public async Task<IManagerActionResult<string>> LoginAsync(LoginRequest login)
    {
        var isCorrect = await _dbInfo.Users.FirstOrDefaultAsync(
            x => x.Login == login.Email && x.Password == login.Password);

        if(isCorrect == null) 
            return new ManagerActionResult<string>(null, ResultEnum.BadRequest, _InvalidLoginOrPasswordMessage);
        else
        {
            var token = _tokenGenerator.GrantToken();
            _tokenStorage.AddToken(token, login.Email);
            return new ManagerActionResult<string>(token, ResultEnum.OK);
        }
    }

    public async Task<IManagerActionResult<string>> RegisterAsync(LoginRequest login) 
    {
        if(!_checker.IsCorrectPassword(login.Password) || !_checker.isCorrectLogin(login.Email))
            return new ManagerActionResult<string>(null, ResultEnum.BadRequest, _InvalidLoginOrPasswordMessage);

        await _dbInfo.Users.AddAsync(new User
        {
            Login = login.Email,
            Password = login.Password
        });
        await _dbInfo.SaveChangesAsync();

        //Error handling?
        
        var token = _tokenGenerator.GrantToken();
        _tokenStorage.AddToken(token, login.Email);
        return new ManagerActionResult<string>(token, ResultEnum.OK);
    }
}