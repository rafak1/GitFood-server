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

    public async Task<IManagerActionResult> AddProductsToFridgeAsync(FridgeAddProductViewModel[] products,int fridgeId, string user)
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
            if(fridge.FridgeProducts.Any(x => x.ProductId == product.ProductId))
            {
                double new_quantity = ((double) fridge.FridgeProducts.First(x => x.ProductId == product.ProductId).Ammount) + product.Quantity;
                if(new_quantity < 0)
                {
                    return new ManagerActionResult(ResultEnum.BadRequest);
                }
                else if(new_quantity == 0)
                {
                    fridge.FridgeProducts.Remove(fridge.FridgeProducts.First(x => x.ProductId == product.ProductId));
                }
                else
                {
                    fridge.FridgeProducts.First(x => x.ProductId == product.ProductId).Ammount = new_quantity;
                }
            }
            else
            {
                fridge.FridgeProducts.Add(new FridgeProduct
                {
                    ProductId = product.ProductId,
                    Ammount = product.Quantity,
                    FridgeId = fridgeId
                });
            }
        }
        await _dbInfo.SaveChangesAsync();
        await transaction.CommitAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<FridgeViewModel>> GetFridgeAsync(int fridgeId, string user)
    {
        var fridge = await _dbInfo.Fridges
            .Include(x => x.Users)
            .Include(x => x.FridgeProducts)
            .ThenInclude(x => x.Product)
            .ThenInclude(x => x.CategoryNavigation)
            .FirstOrDefaultAsync(x => x.Id == fridgeId && (x.UserLogin == user || x.Users.Any(x => x.Login == user)));
        if (fridge is null)
            return new ManagerActionResult<FridgeViewModel>(null, ResultEnum.NotFound);
        return new ManagerActionResult<FridgeViewModel>(ConvertToFridgeViewModel(fridge), ResultEnum.OK);
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
        var transaction = await _dbInfo.Database.BeginTransactionAsync();
        
        var fridgeProducts = await _dbInfo.FridgeProducts.Where(x => x.FridgeId == fridgeId).ToArrayAsync();
        _dbInfo.FridgeProducts.RemoveRange(fridgeProducts);

        var fridge = await _dbInfo.Fridges.FirstOrDefaultAsync(x => x.Id == fridgeId && x.UserLogin == user );
        if (fridge is null){
            transaction.Rollback();
            return new ManagerActionResult(ResultEnum.NotFound);
        }
        await DeleteAllSharedUsersAsync(fridge);
        
        await _dbInfo.Fridges.Where(x => x.Id == fridgeId).ExecuteDeleteAsync();
        await _dbInfo.SaveChangesAsync();
        await transaction.CommitAsync();
            
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<AllFridgesViewModel>> GetAllFridgesAsync(string login)
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
            .Where(x => x.Users.Any(x => x.Login == login))
            .ToArrayAsync();

        var sharedFridges = shared.Select(ConvertToFridgeViewModel).ToArray();
        var fridgesViewModel = fridges.Select(ConvertToFridgeViewModel).ToArray();

        return new ManagerActionResult<AllFridgesViewModel>(new AllFridgesViewModel
        {
            Fridges = fridgesViewModel,
            SharedFridges = sharedFridges
        }, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<FridgeMapEntryViewModel[]>> GetMapForUserAsync(string login)
    {
        var fridges = await _dbInfo.Fridges.Include(x => x.Users).Where(x => x.UserLogin == login).ToArrayAsync();
        var shared = await _dbInfo.Fridges.Include(x => x.Users).Where(x => x.Users.Any(x => x.Login == login)).ToArrayAsync();

        var result = fridges.Select(x => new FridgeMapEntryViewModel
        {
            Id = x.Id,
            Name = x.Name,
            Owner = x.UserLogin,
            isShared = false,
            SharedWith = x.Users.Select(x => x.Login).ToArray()
        }).Concat(shared.Select(x => new FridgeMapEntryViewModel
        {
            Id = x.Id,
            Name = x.Name,
            Owner = x.UserLogin,
            isShared = true,
            SharedWith = x.Users.Select(x => x.Login).ToArray()
        })).ToArray();

        return new ManagerActionResult<FridgeMapEntryViewModel[]>(result, ResultEnum.OK);
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
        var newUser = await _dbInfo.Users.FirstOrDefaultAsync(x => x.Login == userLogin);
        if(newUser is null)
            return new ManagerActionResult(ResultEnum.BadRequest, "User not found");
        fridge.Users.Add(newUser);
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
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

    private FridgeViewModel ConvertToFridgeViewModel(Fridge fridge)
    {
        var fridgeProductsViewModels = fridge.FridgeProducts.Select(x => new FridgeProductViewModel
        {
            Name = x.Product.Name,
            Description = x.Product.Description,
            Barcode = x.Product.Barcode,
            Id = x.Product.Id,
            Quantity = x.Ammount,
            CategoryName = x.Product.CategoryNavigation.Name,
            CategoryStatus = x.Product.CategoryNavigation.Status,
            CategoryUnit = x.Product.CategoryNavigation.Unit,
            CategoryId = x.Product.CategoryNavigation.Id
        }).ToList();

        return new FridgeViewModel
        {
            FridgeId = fridge.Id,
            Name = fridge.Name,
            Owner = fridge.UserLogin,
            Products = fridgeProductsViewModels,
            SharedWith = fridge.Users.Select(x => x.Login).ToArray()
        };
    }

    private async Task DeleteAllSharedUsersAsync(Fridge fridge)
    {
	    _dbInfo.Update(fridge);
        fridge.UserLoginNavigation = null;
	    fridge.Users.Clear();
        await _dbInfo.Database.ExecuteSqlAsync(sql: $"delete from fridge_shares where fridge_id = {fridge.Id}");
        await _dbInfo.SaveChangesAsync();
    }
}
