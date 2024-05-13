using Server.Logic.Abstract.Managers;
using Server.Data.Models;
using Server.ViewModels.Fridge;
using Server.Database;
using Microsoft.EntityFrameworkCore;
using Server.Logic.Abstract;

namespace Server.Logic.Managers;

internal class FridgeManager : IFridgeManager
{
    private readonly GitfoodContext _dbInfo;

    public FridgeManager(GitfoodContext database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task<IManagerActionResult> UpdateProductInFridgeAsync(int fridgeId, int productId, int quantity, string user) 
    {
        var transaction = await _dbInfo.Database.BeginTransactionAsync();

        var fridge = await _dbInfo.Fridges
            .Include(x => x.Users)
            .Include(x => x.FridgeProducts)
            .ThenInclude(x => x.Product)
            .ThenInclude(x => x.CategoryNavigation)
            .FirstOrDefaultAsync(x => x.Id == fridgeId && (x.UserLogin == user || x.Users.Any(x => x.Login == user)));
        if (fridge is null)
            return new ManagerActionResult(ResultEnum.NotFound);

        if(fridge.FridgeProducts.Any(x => x.ProductId == productId))
        {
            if(quantity < 0)
            {
                return new ManagerActionResult(ResultEnum.BadRequest);
            }
            else if(quantity == 0)
            {
                fridge.FridgeProducts.Remove(fridge.FridgeProducts.First(x => x.ProductId == productId));
            }
            else
            {
                fridge.FridgeProducts.First(x => x.ProductId == productId).Ammount = quantity;
            }
        }
        else
        {
            fridge.FridgeProducts.Add(new FridgeProduct
            {
                ProductId = productId,
                Ammount = quantity,
                FridgeId = fridgeId
            });
        }
        await _dbInfo.SaveChangesAsync();
        await transaction.CommitAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult> AddProductsToFridgeAsync((int productId, int quantity)[] products,int fridgeId, string user)
    {
        var transaction = await _dbInfo.Database.BeginTransactionAsync();

        var fridge = await _dbInfo.Fridges
            .Include(x => x.Users)
            .Include(x => x.FridgeProducts)
            .ThenInclude(x => x.Product)
            .ThenInclude(x => x.CategoryNavigation)
            .FirstOrDefaultAsync(x => x.Id == fridgeId);
        if (fridge is null)
            return new ManagerActionResult(ResultEnum.NotFound);

        if (fridge.UserLogin != user)
            return new ManagerActionResult(ResultEnum.Unauthorizated);

        foreach (var product in products)
        {
            if(fridge.FridgeProducts.Any(x => x.ProductId == product.productId))
            {
                double new_quantity = ((double) fridge.FridgeProducts.First(x => x.ProductId == product.productId).Ammount) + product.quantity;
                if(new_quantity < 0)
                {
                    return new ManagerActionResult(ResultEnum.BadRequest);
                }
                else if(new_quantity == 0)
                {
                    fridge.FridgeProducts.Remove(fridge.FridgeProducts.First(x => x.ProductId == product.productId));
                }
                else
                {
                    fridge.FridgeProducts.First(x => x.ProductId == product.productId).Ammount = new_quantity;
                }
            }
            else
            {
                fridge.FridgeProducts.Add(new FridgeProduct
                {
                    ProductId = product.productId,
                    Ammount = product.quantity,
                    FridgeId = fridgeId
                });
            }
        }
        await _dbInfo.SaveChangesAsync();
        await transaction.CommitAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<Fridge>> GetFridgeAsync(int fridgeId, string user)
    {
        var fridge = await _dbInfo.Fridges
            .Include(x => x.Users)
            .Include(x => x.FridgeProducts)
            .ThenInclude(x => x.Product)
            .ThenInclude(x => x.CategoryNavigation)
            .FirstOrDefaultAsync(x => x.Id == fridgeId && (x.UserLogin == user || x.Users.Any(x => x.Login == user)));
        if (fridge is null)
            return new ManagerActionResult<Fridge>(null, ResultEnum.NotFound);
        return new ManagerActionResult<Fridge>(fridge, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<int>> CreateFridgeAsync(string name, string login)
    {
        var transaction = await _dbInfo.Database.BeginTransactionAsync();

        await _dbInfo.Fridges.AddAsync(new Fridge
        {
            Name = name,
            UserLogin = login
        });
        await _dbInfo.SaveChangesAsync();
        var id = (await _dbInfo.Fridges.FirstAsync(x => x.Name == name && x.UserLogin == login)).Id;

        await transaction.CommitAsync();
        return new ManagerActionResult<int>(id, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> DeleteFridgeAsync(int fridgeId, string user)
    {
        var fridge = await _dbInfo.Fridges
            .Include(x => x.FridgeProducts)
            .FirstOrDefaultAsync(x => x.Id == fridgeId && x.UserLogin == user);
            
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<(Fridge[] fridges, Fridge[] shared)>> GetAllFridgesAsync(string login)
    {
        var fridges = await _dbInfo.Fridges
            .Include(x => x.Users)
            .Include(x => x.FridgeProducts)
            .ThenInclude(x => x.Product)
            .ThenInclude(x => x.CategoryNavigation)
            .Where(x => x.UserLogin == login).ToArrayAsync();
        var shared = await _dbInfo.Fridges
            .Include(x => x.Users)
            .Include(x => x.FridgeProducts)
            .ThenInclude(x => x.Product)
            .ThenInclude(x => x.CategoryNavigation)
            .Where(x => x.Users.Any(x => x.Login == login)).ToArrayAsync();
        return new ManagerActionResult<(Fridge[] fridges, Fridge[] shared)>((fridges, shared), ResultEnum.OK);
    }

    public async Task<IManagerActionResult<(int Id, string Name, bool is_shared)[]>> GetMapForUserAsync(string login)
    {
        var fridges = await _dbInfo.Fridges.Where(x => x.UserLogin == login).ToArrayAsync();
        var shared = await _dbInfo.Fridges.Where(x => x.Users.Any(x => x.Login == login)).ToArrayAsync();   


        (int Id, string Name, bool is_shared)[] result = 
            fridges.Select(x => (x.Id, x.Name, false)).ToArray()
            .Concat(shared.Select(x => (x.Id, x.Name, true))).ToArray();

        return new ManagerActionResult<(int Id, string Name, bool is_shared)[]>(result, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> ShareFridgeAsync(int fridgeId, string userLogin, string login)
    {
        var fridge = await _dbInfo.Fridges.
            Include(x => x.Users).
            Include(x => x.UserLoginNavigation).
            FirstOrDefaultAsync(x => x.Id == fridgeId && x.UserLoginNavigation.Login == login);

        if (fridge is null)
            return new ManagerActionResult(ResultEnum.NotFound);

        if (fridge.Users.Any(x => x.Login == userLogin) || fridge.UserLoginNavigation.Login == userLogin)
            return new ManagerActionResult(ResultEnum.BadRequest);
        else 
        {
            fridge.Users.Add(await _dbInfo.Users.FirstAsync(x => x.Login == userLogin));
            await _dbInfo.SaveChangesAsync();
            return new ManagerActionResult(ResultEnum.OK);
        }
    }

    public async Task<IManagerActionResult> UnshareFridgeAsync(int fridgeId, string userLogin, string login)
    {
        var fridge = await _dbInfo.Fridges.
            Include(x => x.Users).
            Include(x => x.UserLoginNavigation).
            FirstOrDefaultAsync(x => x.Id == fridgeId && x.UserLogin == login);

        if (fridge is null)
            return new ManagerActionResult(ResultEnum.NotFound);

        fridge.Users.Remove(await _dbInfo.Users.FirstAsync(x => x.Login == userLogin));
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult> BeUnsharedAsync(int fridgeId, string login)
    {
        var fridge = await _dbInfo.Fridges.
            Include(x => x.Users).
            Include(x => x.UserLoginNavigation).
            FirstOrDefaultAsync(x => x.Id == fridgeId);

        if (fridge is null)
            return new ManagerActionResult(ResultEnum.NotFound);

        fridge.Users.Remove(fridge.Users.First(x => x.Login == login));

        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }
}