using Microsoft.AspNetCore.Identity.Data;

namespace Server.Logic.Abstract.Managers;

public interface ILoginManager
{
    public Task<IManagerActionResult<string>> LoginAsync(LoginRequest login);
    public Task<IManagerActionResult<string>> RegisterAsync(LoginRequest login);
}