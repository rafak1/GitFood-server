using Microsoft.AspNetCore.Mvc;
using Server.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Token;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class CategoryController : BaseController
{
    private const string _controllerRoute = "/category";
    private readonly ICategoryManager _categoryManager;

    public CategoryController(ICategoryManager categoryManager, ITokenStorage tokenStorage) : base(tokenStorage)
    {
        _categoryManager = categoryManager ?? throw new ArgumentNullException(nameof(categoryManager));
    }

    
    [HttpPost]
    [Route($"{_controllerRoute}/addNewRequest")]
    public async Task<IActionResult> AddNewCategoryRequest(CategoryViewModel category)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _categoryManager.AddNewCategoryRequestAsync(category, user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/getAll")]
    public async Task<IActionResult> GetCategoriesAsync() 
        => (await _categoryManager.GetCategoriesAsync()).MapToActionResult();

    [HttpGet]
    [Route($"{_controllerRoute}/getVerified")]
    public async Task<IActionResult> GetVerifiedCategoriesAsync() 
        => (await _categoryManager.GetVerifiedCategoriesAsync()).MapToActionResult();

    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteCategory(int id) 
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user != "0")
            return BadRequest(_userNotFound);
        return (await _categoryManager.DeleteCategoryAsync(id)).MapToActionResult();
    }
}
