using Server.Data.Models;
using Server.ViewModels.Fridge;

namespace Server.Logic.Abstract.Managers;

public interface IFridgeManager
{
    public Task AddProductToFridgeAsync(FridgeProductViewModel fridgeProduct);
    public Task DeleteProductFromFridgeAsync(int fridgeProductId);
    public Task UpdateProductInFridgeAsync(FridgeProductViewModel fridgeProduct);
    public Task<Fridge> GetFridgeAsync(string login);
}