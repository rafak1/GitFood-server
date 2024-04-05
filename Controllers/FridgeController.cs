using Microsoft.AspNetCore.Mvc;
using Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Server.DataModel;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
public class FridgeController : Controller
{
    private const string _controllerRoute = "/fridge";

    private readonly GitfoodContext _dbInfo;

    public FridgeController(GitfoodContext database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }


    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddProductToFridge(FridgeProductViewModel fridgeProduct) 
    {

        if (!Enum.IsDefined(typeof(Units), fridgeProduct.Unit))
        {
            return BadRequest("Invalid Unit");
        }
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
        return Ok();
    }


    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteProductFromFridge(int fridgeProductId) 
    {
        await _dbInfo.FridgeUnits.Where(x => x.FridgeProductId == fridgeProductId).ExecuteDeleteAsync();
        await _dbInfo.Fridges.Where(x => x.Id == fridgeProductId).ExecuteDeleteAsync();
        return Ok();
    }


    [HttpPatch]
    [Route($"{_controllerRoute}/update")]
    public async Task<IActionResult> UpdateProductInFridge(FridgeProductViewModel fridgeProduct) 
    {
        if (!Enum.IsDefined(typeof(Units), fridgeProduct.Unit))
        {
            return BadRequest("Invalid Unit");
        }
        var fridge = await _dbInfo.Fridges.FirstOrDefaultAsync(x => x.ProductId == fridgeProduct.ProductId && x.UserLogin == fridgeProduct.Login);
        if (fridge is null)
        {
            return NotFound();
        }
        if (fridge.FridgeUnits.Any(x => x.Unit == fridgeProduct.Unit))
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
        return Ok();
    }


    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public IActionResult GetFridge(string login) 
    {
        return Ok(_dbInfo.Fridges.Where(x => x.UserLogin == login));
    }

}