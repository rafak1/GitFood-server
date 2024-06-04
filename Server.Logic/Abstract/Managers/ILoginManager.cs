using Server.ViewModels.Login;

namespace Server.Logic.Abstract.Managers;

public interface ILoginManager
{
    public Task<IManagerActionResult<string>> LoginAsync(LoginViewModel login);
    public Task<IManagerActionResult> RegisterAsync(RegisterViewModel login);
    public Task<IManagerActionResult> VerifyAsync(string token, string login);
    public Task<IManagerActionResult> BanAsync(string toBan, string user);
}