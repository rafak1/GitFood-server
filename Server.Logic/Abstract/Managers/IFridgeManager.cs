using Server.Data.Models;
using Server.ViewModels.Fridge;

namespace Server.Logic.Abstract.Managers;

public interface IFridgeManager
{
    public Task<IManagerActionResult> AddProductToFridgeAsync(FridgeProductViewModel fridgeProduct);
    public Task<IManagerActionResult> DeleteProductFromFridgeAsync(int fridgeProductId);
    public Task<IManagerActionResult> UpdateProductInFridgeAsync(FridgeProductViewModel fridgeProduct);
    public Task<IManagerActionResult<Fridge[]>> GetFridgeAsync(string login);
}