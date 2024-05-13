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
            return BadRequest(_userNotFound);

        return (await _fridgeManager.CreateFridgeAsync(name, user)).MapToActionResult();
    }


    [HttpPatch]
    [Route($"{_controllerRoute}/updateProductQuantity")]
    public async Task<IActionResult> UpdateProductInFridge(int fridgeId, int productId, int quantity){
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _fridgeManager.UpdateProductInFridgeAsync(fridgeId, productId, quantity, user)).MapToActionResult();
    }

    [HttpPatch]
    [Route($"{_controllerRoute}/addProducts")]
    public async Task<IActionResult> AddProductsToFridge((int productId, int quantity)[] products, int fridgeId){
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _fridgeManager.AddProductsToFridgeAsync(products,fridgeId,  user)).MapToActionResult();
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/remove")]
    public async Task<IActionResult> RemoveFridge(int fridgeId) 
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _fridgeManager.DeleteFridgeAsync(fridgeId, user)).MapToActionResult();
    }


    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public async Task<IActionResult> GetFridge(int fridgeId)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);
        
        return (await _fridgeManager.GetFridgeAsync(fridgeId, user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/getAll")]
    public async Task<IActionResult> GetAllFridges()
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _fridgeManager.GetAllFridgesAsync(user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/getMap")]
    public async Task<IActionResult> GetFridgeMap()
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _fridgeManager.GetMapForUserAsync(user)).MapToActionResult();
    }

    [HttpPatch]
    [Route($"{_controllerRoute}/share")]
    public async Task<IActionResult> ShareFridge(int fridgeId, string userLogin)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _fridgeManager.ShareFridgeAsync(fridgeId, userLogin, user)).MapToActionResult();
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/unshare")]
    public async Task<IActionResult> UnshareFridge(int fridgeId, string userLogin)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _fridgeManager.UnshareFridgeAsync(fridgeId, userLogin, user)).MapToActionResult();
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/beUnshared")]
    public async Task<IActionResult> BeUnshared(int fridgeId)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _fridgeManager.BeUnsharedAsync(fridgeId, user)).MapToActionResult();
    }
}