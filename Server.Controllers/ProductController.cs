using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Token;
using Server.ViewModels.Products;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class ProductController : Controller
{
    private readonly IProductManager _productManager;

    private readonly ITokenStorage _tokenStorage;

    private const string ControllerPart = "/products";

    private const int _bearerOffset = 7;

    public ProductController(IProductManager productManager, ITokenStorage tokenStorage)
    {
        _productManager = productManager ?? throw new ArgumentNullException(nameof(productManager));
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
    }

    [HttpPost]
    [Route($"{ControllerPart}/add")]
    public async Task<IActionResult> AddProduct(ProductViewModel product) 
        => (await _productManager.AddProductAsync(product)).MapToActionResult();

    [HttpPost]
    [Route($"{ControllerPart}/addWithBarcode")]
    public async Task<IActionResult> AddProductWithBarcode(ProductWithBarcodeViewModel product)
    { 
        var user = _tokenStorage.GetUser(Request.Headers.Authorization.ToString()[_bearerOffset..]);
        if(user == null) 
            return BadRequest("No user found assigned to this token");

        return (await _productManager.AddProductWithBarcodeAsync(product, user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{ControllerPart}/getByBarcode")]
    public async Task<IActionResult> GetProductByBarcode(String barcode) 
    {
        var user = _tokenStorage.GetUser(Request.Headers.Authorization.ToString()[_bearerOffset..]);
        if(user == null) 
            return BadRequest("No user found assigned to this token");

        return (await _productManager.GetProductByBarcodeAsync(barcode, user)).MapToActionResult();
    }

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
