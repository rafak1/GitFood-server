

using Microsoft.EntityFrameworkCore;
using Server.Data.Models;
using Server.Database;
using Server.Logic.Abstract;
using Server.Logic.Abstract.Managers;

namespace Server.Logic.Managers;

public class ShoppingListManager : IShoppingListManager
{
    private readonly GitfoodContext _dbInfo;

    public ShoppingListManager(GitfoodContext dbInfo) 
    {
        _dbInfo = dbInfo ?? throw new ArgumentNullException(nameof(dbInfo));
    }

    public async Task<IManagerActionResult<int>> CreateShoppingListAsync(string name, string user)
    {
        var transaction = _dbInfo.Database.BeginTransaction();
        await _dbInfo.ShoppingLists.AddAsync(new ShoppingList {
            Name = name,
            User = user
        });
        await _dbInfo.SaveChangesAsync();
        var id = (await _dbInfo.ShoppingLists.Where(x => x.User == user).FirstAsync()).Id;
        await transaction.CommitAsync();
        return new ManagerActionResult<int>(id, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> DeleteShoppingListAsync(int shoppingListId)
    {
        await _dbInfo.ShoppingLists.Where(x => x.Id == shoppingListId).ExecuteDeleteAsync();
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<ShoppingList[]>> GetAllShoppingListsAsync(string user)
    {
        var shoppingLists = await _dbInfo.ShoppingLists.Where(x => x.User == user).ToArrayAsync();
        return new ManagerActionResult<ShoppingList[]>(shoppingLists, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<ShoppingList>> GetShoppingListAsync(int shoppingListId)
    {
        var shoppingList = await _dbInfo.ShoppingLists.FirstOrDefaultAsync(x => x.Id == shoppingListId);
        return new ManagerActionResult<ShoppingList>(shoppingList, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> UpdateShoppingListAsync(int shoppingListId, int categoryId, int quantity)
    {
        var transaction = _dbInfo.Database.BeginTransaction();
        var shoppingList = _dbInfo.ShoppingLists.FirstOrDefault(x => x.Id == shoppingListId);
        if (shoppingList is null)
            return new ManagerActionResult(ResultEnum.NotFound);
        if (shoppingList.ShoppingListProducts.Any(x => x.Category == categoryId))
        {
            var shoppingListProduct = shoppingList.ShoppingListProducts.First(x => x.Category == categoryId);
            shoppingListProduct.Quantity = quantity;
            if(quantity == 0)
                shoppingList.ShoppingListProducts.Remove(shoppingListProduct);
        }
        else
        {
            shoppingList.ShoppingListProducts.Add(new ShoppingListProduct {
                Category = categoryId,
                Quantity = quantity
            });
        }
        await _dbInfo.SaveChangesAsync();
        await transaction.CommitAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }
}