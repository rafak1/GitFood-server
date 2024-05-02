
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

}