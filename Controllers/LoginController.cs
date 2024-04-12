using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.DataModel;

[ApiController]
public class LoginController : Controller{

    private const string _controllerRoute = "/login";

    private readonly GitfoodContext _dbInfo;

    private readonly ITokenGenerator _tokenGenerator;

    private readonly IConfiguration _config;

    public LoginController(GitfoodContext database, ITokenGenerator tokenGenerator, IConfiguration config)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
        _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    [HttpPost]
    [AllowAnonymous]
    [Route($"{_controllerRoute}")]
    public async Task<IActionResult> Login(LoginRequest login)
    {
        var isCorrect = await _dbInfo.Users.FirstOrDefaultAsync(
            x => x.Login == login.Email && x.Password == login.Password);
        return isCorrect is null ? Unauthorized("") : Ok(_tokenGenerator.GrantToken(_config["Jwt:Key"], _config["Jwt:Issuer"]));
    }

    [HttpPost]
    [AllowAnonymous]
    [Route($"{_controllerRoute}/register")]
    public async Task<IActionResult> Register(LoginRequest login) 
    {
        await _dbInfo.Users.AddAsync(new User
        {
            Login = login.Email,
            Password = login.Password
        });
        await _dbInfo.SaveChangesAsync();

        //Error handling?

        return Ok(_tokenGenerator.GrantToken(_config["Jwt:Key"], _config["Jwt:Issuer"]));
    }

}