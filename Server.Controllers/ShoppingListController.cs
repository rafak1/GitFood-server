using Microsoft.AspNetCore.Mvc;
using Server.ViewModels.Fridge;
using Microsoft.AspNetCore.Authorization;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Token;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class ShoppingListController : BaseController
{
    private const string _controllerRoute = "/shoppingList";
    private readonly IShoppingListManager _shoppingListManager;

    public ShoppingListController(IShoppingListManager shoppingListManager, ITokenStorage tokenStorage) : base(tokenStorage)
    {
        _shoppingListManager = shoppingListManager ?? throw new ArgumentNullException(nameof(shoppingListManager));
    }

    [HttpPost]
    [Route($"{_controllerRoute}/create")]
    public async Task<IActionResult> CreateShoppingList(string name) 
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _shoppingListManager.CreateShoppingListAsync(name, user)).MapToActionResult();
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/remove")]
    public async Task<IActionResult> RemoveShoppingList(int shoppingListId) 
        => (await _shoppingListManager.DeleteShoppingListAsync(shoppingListId)).MapToActionResult();

    [HttpGet]
    [Route($"{_controllerRoute}/getAll")]
    public async Task<IActionResult> GetAllShoppingLists() 
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _shoppingListManager.GetAllShoppingListsAsync(user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public async Task<IActionResult> GetShoppingList(int shoppingListId) 
        => (await _shoppingListManager.GetShoppingListAsync(shoppingListId)).MapToActionResult();

    [HttpPatch]
    [Route($"{_controllerRoute}/update")]
    public async Task<IActionResult> UpdateShoppingList(int shoppingListId, int categoryId, int quantity) 
        => (await _shoppingListManager.UpdateShoppingListAsync(shoppingListId, categoryId, quantity)).MapToActionResult();
    

    [HttpGet]
    [Route($"{_controllerRoute}/getMap")]
    public async Task<IActionResult> GetShoppingListMap() 
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _shoppingListManager.GetShoppingListMapAsync(user)).MapToActionResult();
    }
}