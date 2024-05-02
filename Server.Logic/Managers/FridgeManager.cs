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

        var fridge = await _dbInfo.Fridges.FirstOrDefaultAsync(x => x.Id == fridgeId);
        if (fridge is null)
            return new ManagerActionResult(ResultEnum.NotFound);

        if(fridge.FridgeProducts.Any(x => x.ProductId == productId))
        {
            var currentAmmount = fridge.FridgeProducts.First(x => x.ProductId == productId).Ammount;
            if(currentAmmount + quantity < 0)
            {
                return new ManagerActionResult(ResultEnum.BadRequest);
            }
            else if(currentAmmount + quantity == 0)
            {
                fridge.FridgeProducts.Remove(fridge.FridgeProducts.First(x => x.ProductId == productId));
            }
            else
            {
                fridge.FridgeProducts.First(x => x.ProductId == productId).Ammount += quantity;
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

    public async Task<IManagerActionResult<Fridge>> GetFridgeAsync(int fridgeId)
    {
        var fridge = await _dbInfo.Fridges.FirstOrDefaultAsync(x => x.Id == fridgeId);
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

    public async Task<IManagerActionResult> DeleteFridgeAsync(int fridgeId)
    {
        await _dbInfo.Fridges.Where(x => x.Id == fridgeId).ExecuteDeleteAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<Fridge[]>> GetAllFridgesAsync(string login)
    {
        var fridges = await _dbInfo.Fridges.Where(x => x.UserLogin == login).ToArrayAsync();
        return new ManagerActionResult<Fridge[]>(fridges, ResultEnum.OK);
    }
}