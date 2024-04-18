using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Server.Logic.Abstract.Managers;

namespace Server.Controllers;

[ApiController]
public class LoginController : Controller
{

    private const string _controllerRoute = "/login";
    private readonly ILoginManager _loginManager;

    public LoginController(ILoginManager loginManager)
    {
        _loginManager = loginManager ?? throw new ArgumentNullException(nameof(loginManager));
    }

    [HttpPost]
    [AllowAnonymous]
    [Route($"{_controllerRoute}")]
    public async Task<IActionResult> Login(LoginRequest login)
        => (await _loginManager.LoginAsync(login)).MapToActionResult();

    [HttpPost]
    [AllowAnonymous]
    [Route($"{_controllerRoute}/register")]
    public async Task<IActionResult> Register(LoginRequest login) 
        => (await _loginManager.RegisterAsync(login)).MapToActionResult();
}
