using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Data.Models;

[ApiController]
public class LoginController : Controller{

    private const string _controllerRoute = "/login";

    private const string _InvalidLoginOrPasswordMessage = "Invalid login or password";

    private readonly GitfoodContext _dbInfo;

    private readonly ITokenGenerator _tokenGenerator;

    private readonly IPasswordChecker _checker;

    private readonly ITokenStorage _tokenStorage;

    public LoginController(GitfoodContext database, ITokenGenerator tokenGenerator, IPasswordChecker checker, ITokenStorage tokenStorage)
    {
        _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
        _checker = checker ?? throw new ArgumentNullException(nameof(checker));
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
    }

    [HttpPost]
    [AllowAnonymous]
    [Route($"{_controllerRoute}")]
    public async Task<IActionResult> Login(LoginRequest login)
    {
        var isCorrect = await _dbInfo.Users.FirstOrDefaultAsync(
            x => x.Login == login.Email && x.Password == login.Password);

        if(isCorrect == null) return BadRequest(_InvalidLoginOrPasswordMessage);
        else
        {
            var token = _tokenGenerator.GrantToken();
            _tokenStorage.AddToken(token, login.Email);
            return Ok(token);
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [Route($"{_controllerRoute}/register")]
    public async Task<IActionResult> Register(LoginRequest login) 
    {
        if(!_checker.IsCorrectPassword(login.Password) || !_checker.isCorrectLogin(login.Email))
            return BadRequest(_InvalidLoginOrPasswordMessage);

        await _dbInfo.Users.AddAsync(new User
        {
            Login = login.Email,
            Password = login.Password
        });
        await _dbInfo.SaveChangesAsync();

        //Error handling?
        
        var token = _tokenGenerator.GrantToken();
        _tokenStorage.AddToken(token, login.Email);
        return Ok(token);
    }
}
