using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Server.DataModel;
using Server.ViewModels;

namespace Server.Controllers;

[ApiController]
public class ProductController : Controller
{
    private readonly GitfoodContext _dbInfo;
    private const string ControllerPart = "products";

    public ProductController(GitfoodContext database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    [HttpPost]
    [Route($"{ControllerPart}/add")]
    public async Task<IActionResult> AddProduct(ProductViewModel product) 
    {
        await _dbInfo.Products.AddAsync(new Product{
            Name = product.Name,
            Description = product.Description
        });
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    [Route($"{ControllerPart}/get")]
    public IActionResult GetProduct(string name) 
    {
        return Ok(_dbInfo.Products.Where(x => x.Name.Contains(name)));
    }

    [HttpGet]
    [Route($"{ControllerPart}/getById")]
    public IActionResult GetProductById(int id) 
    {
        return Ok(_dbInfo.Products.Where(x => x.Id == id));
    }

    [HttpDelete]
    [Route($"{ControllerPart}/delete")]
    public async Task<IActionResult> DeleteProduct(int id) 
    {
        await _dbInfo.Products.Where(x => x.Id == id).ExecuteDeleteAsync();
        return Ok();
    }

    [HttpPost]
    [Route($"{ControllerPart}/addToCategories")]
    public async Task<IActionResult> AddCategoriesToProduct(ProductToCategoriesViewModel model) 
    {
        var product = await _dbInfo.Products.FirstOrDefaultAsync(x => x.Id == model.ProductId);
        _dbInfo.Products.Update(product);
        if(product is null || model.CategoriesIds is null)
            return BadRequest();
        product.Categories.AddRange(model.CategoriesIds.Select(x => new Category(){ Id = x }));
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

}
