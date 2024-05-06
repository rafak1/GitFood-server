
using Server.Data.Models;

namespace Server.Logic.Abstract.Managers;

public interface IShoppingListManager
{
    public Task<IManagerActionResult<int>> CreateShoppingListAsync(string name, string user);
    public Task<IManagerActionResult> DeleteShoppingListAsync(int shoppingListId);
    public Task<IManagerActionResult<ShoppingList[]>> GetAllShoppingListsAsync(string user);
    public Task<IManagerActionResult<ShoppingList>> GetShoppingListAsync(int shoppingListId);
    public Task<IManagerActionResult> UpdateShoppingListAsync(int shoppingListId, int categoryId, int quantity);
    public Task<IManagerActionResult<(int Id, string Name)[]>> GetShoppingListMapAsync(string user);
}