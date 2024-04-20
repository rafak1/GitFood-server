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

    public async Task<IManagerActionResult> AddProductToFridgeAsync(FridgeProductViewModel fridgeProduct) 
    {

        if (!Enum.IsDefined(typeof(Units), fridgeProduct.Unit))
            return new ManagerActionResult(ResultEnum.BadRequest);

        await _dbInfo.Fridges.AddAsync(new Fridge
        {
            ProductId = fridgeProduct.ProductId,
            UserLogin = fridgeProduct.Login,
            FridgeUnits =
            [
                new FridgeUnit
                {
                    Unit = fridgeProduct.Unit,
                    Quantity = fridgeProduct.Quantity
                }
            ]
        });
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult> DeleteProductFromFridgeAsync(int fridgeProductId) 
    {
        await _dbInfo.FridgeUnits.Where(x => x.FridgeProductId == fridgeProductId).ExecuteDeleteAsync();
        await _dbInfo.Fridges.Where(x => x.Id == fridgeProductId).ExecuteDeleteAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult> UpdateProductInFridgeAsync(FridgeProductViewModel fridgeProduct) 
    {
        if (!Enum.IsDefined(typeof(Units), fridgeProduct.Unit))
            return new ManagerActionResult(ResultEnum.BadRequest);

        var fridge = await _dbInfo.Fridges.FirstOrDefaultAsync(x => x.ProductId == fridgeProduct.ProductId && x.UserLogin == fridgeProduct.Login);
        if (fridge is null)
            return new ManagerActionResult(ResultEnum.NotFound);

        if(fridge.FridgeUnits.Any(x => x.Unit == fridgeProduct.Unit))
        {
            fridge.FridgeUnits.First(x => x.Unit == fridgeProduct.Unit).Quantity = fridgeProduct.Quantity;
        }
        else
        {
            fridge.FridgeUnits.Add(new FridgeUnit
            {
                Unit = fridgeProduct.Unit,
                Quantity = fridgeProduct.Quantity
            });
        }
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<Fridge>> GetFridgeAsync(string login)
    {
        var fridge = await _dbInfo.Fridges.FirstOrDefaultAsync(x => x.UserLogin == login);
        return new ManagerActionResult<Fridge>(fridge, ResultEnum.OK);
    }
}