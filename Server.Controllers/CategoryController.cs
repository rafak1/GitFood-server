using Microsoft.AspNetCore.Mvc;
using Server.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Server.Logic.Abstract.Managers;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class CategoryController : Controller
{
    private const string _controllerRoute = "/category";
    private readonly ICategoryManager _categoryManager;

    public CategoryController(ICategoryManager categoryManager)
    {
        _categoryManager = categoryManager ?? throw new ArgumentNullException(nameof(categoryManager));
    }

    
    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddCategory(CategoryViewModel category) 
    {
        await _categoryManager.AddCategoryAsync(category);
        return Ok();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public async Task<IActionResult> GetCategoriesAsync(string name) 
    {
        return Ok(await _categoryManager.GetCategoriesAsync(name));
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteCategory(int id) 
    {
        await _categoryManager.DeleteCategoryAsync(id);
        return Ok();
    }
}
