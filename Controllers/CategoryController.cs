using Microsoft.AspNetCore.Mvc;
using Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Server.DataModel;

namespace Server.Controllers;

[ApiController]
public class CategoryController : Controller
{
    private const string _controllerRoute = "/category";

    private readonly GitfoodContext _dbInfo;

    public CategoryController(GitfoodContext database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    
    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddCategory(CategoryViewModel category) 
    {
        await _dbInfo.Categories.AddAsync(new Category() {
            Name = category.Name,
        });
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public IActionResult GetCategories(string name) 
    {
        return Ok(_dbInfo.Categories.Where(x => x.Name.Contains(name)));
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteCategory(int id) 
    {
        await _dbInfo.Categories.Where(x => x.Id == id).ExecuteDeleteAsync();
        return Ok();
    }
}
