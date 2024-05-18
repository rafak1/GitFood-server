
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Token;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class FoodCategoriesController : BaseController
{
    private const string _controllerRoute = "/foodCategories";

    private readonly IFoodCategoryManager _foodCategoryManager;

    public FoodCategoriesController(IFoodCategoryManager foodCategoryManager, ITokenStorage tokenStorage) : base(tokenStorage)
    {
        _foodCategoryManager = foodCategoryManager ?? throw new ArgumentNullException(nameof(foodCategoryManager));
    }

    [HttpGet]
    [Route($"{_controllerRoute}/getAll")]
    public async Task<IActionResult> GetFoodCategoriesAsync()
        => (await _foodCategoryManager.GetAllFoodCategoriesAsync()).MapToActionResult();

    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddFoodCategoryAsync(string name, string descritption)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _foodCategoryManager.AddFoodCategoryAsync(name, descritption, user)).MapToActionResult();
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/remove")]
    public async Task<IActionResult> RemoveFoodCategoryAsync(int foodCategoryId)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _foodCategoryManager.RemoveFoodCategoryAsync(foodCategoryId, user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/getSuggestions")]
    public async Task<IActionResult> GetFoodCategorySuggestionsAsync(string name, int resultsCount)
        => (await _foodCategoryManager.GetFoodCategorySuggestionsAsync(name, resultsCount)).MapToActionResult();

}