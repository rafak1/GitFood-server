using Microsoft.AspNetCore.Identity.Data;

namespace Server.Logic.Abstract.Managers;

public interface ILoginManager
{
    public Task<string> LoginAsync(LoginRequest login);
    public Task<string> RegisterAsync(LoginRequest login);
}