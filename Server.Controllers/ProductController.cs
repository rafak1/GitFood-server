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
        => (await _productManager.AddProductAsync(product)).MapToActionResult();

    [HttpGet]
    [Route($"{ControllerPart}/get")]
    public async Task<IActionResult> GetProducts(string name) 
        => (await _productManager.GetProductsAsync(name)).MapToActionResult();

    [HttpGet]
    [Route($"{ControllerPart}/getById")]
    public async Task<IActionResult> GetProductById(int id) 
        => (await _productManager.GetProductByIdAsync(id)).MapToActionResult();

    [HttpDelete]
    [Route($"{ControllerPart}/delete")]
    public async Task<IActionResult> DeleteProduct(int id) 
        => (await _productManager.DeleteProductAsync(id)).MapToActionResult();

    [HttpPost]
    [Route($"{ControllerPart}/addToCategories")]
    public async Task<IActionResult> AddCategoriesToProduct(ProductToCategoriesViewModel model) 
        => (await _productManager.AddCategoriesToProductAsync(model)).MapToActionResult();
}
