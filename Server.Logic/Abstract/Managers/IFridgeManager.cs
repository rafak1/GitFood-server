using Server.Data.Models;
using Server.ViewModels.Fridge;

namespace Server.Logic.Abstract.Managers;

public interface IFridgeManager
{
    public Task<IManagerActionResult<int>> CreateFridgeAsync(string name, string login);
    public Task<IManagerActionResult> UpdateProductInFridgeAsync(int fridgeId, int productId, int quantity, string user);
    public Task<IManagerActionResult> DeleteFridgeAsync(int fridgeId, string user);
    public Task<IManagerActionResult<FridgeViewModel>> GetFridgeAsync(int fridgeId, string user);
    public Task<IManagerActionResult<AllFridgesViewModel>> GetAllFridgesAsync(string login);
    public Task<IManagerActionResult<FridgeMapEntryViewModel[]>> GetMapForUserAsync(string login);
    public Task<IManagerActionResult> AddProductsToFridgeAsync(FridgeAddProductViewModel[] products,int fridgeId, string user);
    public Task<IManagerActionResult> ShareFridgeAsync(int fridgeId, string userLogin, string login);
    public Task<IManagerActionResult> UnshareFridgeAsync(int fridgeId, string userLogin, string login);
    public Task<IManagerActionResult> BeUnsharedAsync(int fridgeId, string login);
}