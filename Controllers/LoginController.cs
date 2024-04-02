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

    private readonly IConfiguration _config;

    public LoginController(GitfoodContext database, IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    [HttpPost]
    [AllowAnonymous]
    [Route($"{_controllerRoute}")]
    public async Task<IActionResult> Login(LoginRequest login)
    {
        var isCorrect = await _dbInfo.Users.FirstOrDefaultAsync(
            x => x.Login == login.Email && x.Password == login.Password);
        return isCorrect is null ? Unauthorized("") : Ok(GrantToken());
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

        return Ok(GrantToken());
    }

    private string GrantToken(){
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            null,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);


        return new JwtSecurityTokenHandler().WriteToken(Sectoken);
    }

}