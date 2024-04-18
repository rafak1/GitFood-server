using Microsoft.AspNetCore.Mvc;
using Server.ViewModels.Fridge;
using Server.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Server.Logic.Abstract.Managers;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class FridgeController : Controller
{
    private const string _controllerRoute = "/fridge";
    private readonly IFridgeManager _fridgeManager;

    public FridgeController(IFridgeManager fridgeManager)
    {
        _fridgeManager = fridgeManager ?? throw new ArgumentNullException(nameof(fridgeManager));
    }

    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddProductToFridge(FridgeProductViewModel fridgeProduct) 
    {
        await _fridgeManager.AddProductToFridgeAsync(fridgeProduct);
        return Ok();
    }


    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteProductFromFridge(int fridgeProductId) 
    {
        await _fridgeManager.DeleteProductFromFridgeAsync(fridgeProductId);
        return Ok();
    }


    [HttpPatch]
    [Route($"{_controllerRoute}/update")]
    public async Task<IActionResult> UpdateProductInFridge(FridgeProductViewModel fridgeProduct) 
    {
        await _fridgeManager.UpdateProductInFridgeAsync(fridgeProduct);
        return Ok();
    }


    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public async Task<IActionResult> GetFridge(string login) 
    {
        return Ok(await _fridgeManager.GetFridgeAsync(login));
    }

}