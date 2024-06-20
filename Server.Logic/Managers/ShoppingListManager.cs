

using Microsoft.EntityFrameworkCore;
using Server.Data.Models;
using Server.Database;
using Server.Logic.Abstract;
using Server.Logic.Abstract.Managers;

namespace Server.Logic.Managers;

public class ShoppingListManager : IShoppingListManager
{
    private readonly GitfoodContext _dbInfo;

    private static readonly string _recipeNotFound = "Recipe not found";

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
        var transaction = _dbInfo.Database.BeginTransaction();

        var shoppingListProducts = await _dbInfo.ShoppingListProducts.Where(x => x.ShoppingListId == shoppingListId).ToArrayAsync();
        _dbInfo.ShoppingListProducts.RemoveRange(shoppingListProducts);
        
        await _dbInfo.SaveChangesAsync();

        var shoppingList = await _dbInfo.ShoppingLists.FirstOrDefaultAsync(x => x.Id == shoppingListId);
        _dbInfo.ShoppingLists.Remove(shoppingList);

        await _dbInfo.SaveChangesAsync();
        await transaction.CommitAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<ShoppingList[]>> GetAllShoppingListsAsync(string user)
    {
        var shoppingLists = await _dbInfo.ShoppingLists.Where(x => x.User == user)
            .Include(x => x.ShoppingListProducts)
            .ThenInclude(x => x.CategoryNavigation)
            .ToArrayAsync();
        return new ManagerActionResult<ShoppingList[]>(shoppingLists, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<ShoppingList>> GetShoppingListAsync(int shoppingListId)
    {
        var shoppingList = await _dbInfo.ShoppingLists
            .Include(x => x.ShoppingListProducts)
            .ThenInclude(x => x.CategoryNavigation)
            .FirstOrDefaultAsync(x => x.Id == shoppingListId);
        return new ManagerActionResult<ShoppingList>(shoppingList, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<(int Id, string Name)[]>> GetShoppingListMapAsync(string user)
    {
        var shoppingLists = await _dbInfo.ShoppingLists.Where(x => x.User == user).ToArrayAsync();

        (int Id, string Name)[] shoppingListMap = shoppingLists.Select(x => (x.Id, x.Name)).ToArray();

        return new ManagerActionResult<(int Id, string Name)[]>(shoppingListMap, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> UpdateShoppingListAsync(int shoppingListId, int categoryId, double quantity)
    {
        var transaction = _dbInfo.Database.BeginTransaction();
        var shoppingList = _dbInfo.ShoppingLists
            .Include(x => x.ShoppingListProducts)
            .ThenInclude(x => x.CategoryNavigation)
            .FirstOrDefault(x => x.Id == shoppingListId);
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


    public async Task<IManagerActionResult<int>> CreateShoppingListByRecipeAsync(int recipeId, int[] fridgesId, string user)
    {
        var fridges = await _dbInfo.Fridges.Where(x => fridgesId.Contains(x.Id)).ToArrayAsync();
        var recipe = await _dbInfo.Recipes
            .Include(x => x.RecipiesIngredients)
            .ThenInclude(x => x.CategoryNavigation)
            .FirstOrDefaultAsync(x => x.Id == recipeId);

        if (recipe is null)
            return new ManagerActionResult<int>(-1, ResultEnum.BadRequest, _recipeNotFound);

        (double Quantity, int Category)[] shoppingListProducts = recipe.RecipiesIngredients
            .Where(x => x.Quantity != null)
            .Select(x => (x.Quantity.Value, x.Category))
            .ToArray();

        foreach (var fridge in fridges)
        {
            var fridgeProducts = _dbInfo.FridgeProducts
                .Include(x => x.Product)
                .ThenInclude(x => x.CategoryNavigation)
                .Where(x => x.FridgeId == fridge.Id)
                .Select(x => new {x.Product.Category, x.Ammount});
                
            foreach (var fridgeProduct in fridgeProducts)
            {
                var shoppingListProduct = shoppingListProducts.FirstOrDefault(x => x.Category == fridgeProduct.Category);
                if (shoppingListProduct.Quantity == 0)
                    continue;
                shoppingListProduct.Quantity -= fridgeProduct.Ammount ?? 0;
                if (shoppingListProduct.Quantity <= 0)
                    shoppingListProducts = shoppingListProducts.Where(x => x.Category != fridgeProduct.Category).ToArray();
            }
        }

        var shoppingList = new ShoppingList {
            Name = recipe.Name,
            User = user
        };

        foreach (var shoppingListProduct in shoppingListProducts)
        {
            shoppingList.ShoppingListProducts.Add(new ShoppingListProduct {
                Category = shoppingListProduct.Category,
                Quantity = shoppingListProduct.Quantity
            });
        }
        
        var res = await _dbInfo.ShoppingLists.AddAsync(shoppingList);
        await _dbInfo.SaveChangesAsync();

        return new ManagerActionResult<int>(res.Entity.Id , ResultEnum.OK);
    }
}
