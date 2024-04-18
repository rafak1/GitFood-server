using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Logic.Abstract.Managers;
using Server.ViewModels.Products;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class ProductController : Controller
{
    private readonly IProductManager _productManager;
    private const string ControllerPart = "/products";

    public ProductController(IProductManager productManager)
    {
        _productManager = productManager ?? throw new ArgumentNullException(nameof(productManager));
    }

    [HttpPost]
    [Route($"{ControllerPart}/add")]
    public async Task<IActionResult> AddProduct(ProductViewModel product) 
    {
        await _productManager.AddProductAsync(product);
        return Ok();
    }

    [HttpGet]
    [Route($"{ControllerPart}/get")]
    public async Task<IActionResult> GetProducts(string name) 
    {
        return Ok(await _productManager.GetProductsAsync(name));
    }

    [HttpGet]
    [Route($"{ControllerPart}/getById")]
    public async Task<IActionResult> GetProductById(int id) 
    {
        return Ok(await _productManager.GetProductByIdAsync(id));
    }

    [HttpDelete]
    [Route($"{ControllerPart}/delete")]
    public async Task<IActionResult> DeleteProduct(int id) 
    {
        await _productManager.DeleteProductAsync(id);
        return Ok();
    }

    [HttpPost]
    [Route($"{ControllerPart}/addToCategories")]
    public async Task<IActionResult> AddCategoriesToProduct(ProductToCategoriesViewModel model) 
    {
        await _productManager.AddCategoriesToProductAsync(model);
        return Ok();
    }
}
