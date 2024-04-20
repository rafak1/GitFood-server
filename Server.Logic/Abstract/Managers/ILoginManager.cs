using Server.ViewModels.Login;

namespace Server.Logic.Abstract.Managers;

public interface ILoginManager
{
    public Task<IManagerActionResult<string>> LoginAsync(LoginViewModel login);
    public Task<IManagerActionResult<string>> RegisterAsync(LoginViewModel login);
}