using Microsoft.AspNetCore.Mvc;
using Server.ViewModels.Fridge;
using Microsoft.AspNetCore.Authorization;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Token;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class FridgeController : BaseController
{
    private const string _controllerRoute = "/fridge";
    private readonly IFridgeManager _fridgeManager;

    public FridgeController(IFridgeManager fridgeManager, ITokenStorage tokenStorage) : base(tokenStorage)
    {
        _fridgeManager = fridgeManager ?? throw new ArgumentNullException(nameof(fridgeManager));
    }

    [HttpPost]
    [Route($"{_controllerRoute}/create")]
    public async Task<IActionResult> CreateFridge(string name) 
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest("No user found assigned to this token");

        return (await _fridgeManager.CreateFridgeAsync(name, user)).MapToActionResult();
    }


    [HttpPatch]
    [Route($"{_controllerRoute}/updateProductQuantity")]
    public async Task<IActionResult> UpdateProductInFridge(int fridgeId, int productId, int quantity){
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest("No user found assigned to this token");

        return (await _fridgeManager.UpdateProductInFridgeAsync(fridgeId, productId, quantity, user)).MapToActionResult();
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/remove")]
    public async Task<IActionResult> RemoveFridge(int fridgeId) 
        => (await _fridgeManager.DeleteFridgeAsync(fridgeId)).MapToActionResult();


    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public async Task<IActionResult> GetFridge(int fridgeId) 
        => (await _fridgeManager.GetFridgeAsync(fridgeId)).MapToActionResult();

    [HttpGet]
    [Route($"{_controllerRoute}/getAll")]
    public async Task<IActionResult> GetAllFridges(){
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest("No user found assigned to this token");

        return (await _fridgeManager.GetAllFridgesAsync(user)).MapToActionResult();
    }

}