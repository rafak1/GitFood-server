using Server.Data.Models;
using Server.ViewModels.Fridge;

namespace Server.Logic.Abstract.Managers;

public interface IFridgeManager
{
    public Task<IManagerActionResult<int>> CreateFridgeAsync(string name, string login);
    public Task<IManagerActionResult> UpdateProductInFridgeAsync(int fridgeId, int productId, int quantity, string user);
    public Task<IManagerActionResult> DeleteFridgeAsync(int fridgeId);
    public Task<IManagerActionResult<Fridge>> GetFridgeAsync(int fridgeId);
    public Task<IManagerActionResult<Fridge[]>> GetAllFridgesAsync(string login);
}