using Server.Logic.Abstract.Managers;
using Server.Data.Models;
using Server.ViewModels.Fridge;
using Server.Database;
using Microsoft.EntityFrameworkCore;

namespace Server.Logic.Managers;

internal class FridgeManager : IFridgeManager
{
    private readonly GitfoodContext _dbInfo;

    public FridgeManager(GitfoodContext database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task AddProductToFridgeAsync(FridgeProductViewModel fridgeProduct) 
    {

        if (!Enum.IsDefined(typeof(Units), fridgeProduct.Unit))
            return; //BadRequest
            
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
        // OK
    }

    public async Task DeleteProductFromFridgeAsync(int fridgeProductId) 
    {
        await _dbInfo.FridgeUnits.Where(x => x.FridgeProductId == fridgeProductId).ExecuteDeleteAsync();
        await _dbInfo.Fridges.Where(x => x.Id == fridgeProductId).ExecuteDeleteAsync();
        // OK
    }

    public async Task UpdateProductInFridgeAsync(FridgeProductViewModel fridgeProduct) 
    {
        if (!Enum.IsDefined(typeof(Units), fridgeProduct.Unit))
            return; // BadRequest

        var fridge = await _dbInfo.Fridges.FirstOrDefaultAsync(x => x.ProductId == fridgeProduct.ProductId && x.UserLogin == fridgeProduct.Login);
        if (fridge is null)
            return; // NotFound

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
        // OK
    }

    public async Task<Fridge> GetFridgeAsync(string login)
        => await _dbInfo.Fridges.FirstOrDefaultAsync(x => x.UserLogin == login);
}