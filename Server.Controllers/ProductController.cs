using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Token;
using Server.ViewModels.Products;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class ProductController : BaseController
{
    private const string _controllerRoute = "/product";

    private readonly IProductManager _productManager;

    public ProductController(IProductManager productManager, ITokenStorage tokenStorage) : base(tokenStorage)
    {
        _productManager = productManager ?? throw new ArgumentNullException(nameof(productManager));
    }

    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddProduct(ProductViewModel product)
    {
        var user = GetUser();
        return (await _productManager.AddProductAsync(product, user)).MapToActionResult();
    }

    [HttpPatch]
    [Route($"{_controllerRoute}/update")]
    public async Task<IActionResult> UpdateProduct(ProductViewModel product, int id)
    {
        var user = GetUser();
        return (await _productManager.UpdateProductAsync(product, user, id)).MapToActionResult();
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteProduct(int id)
        => (await _productManager.DeleteProductAsync(id)).MapToActionResult();

    [HttpGet]
    [Route($"{_controllerRoute}/getById")]
    public async Task<IActionResult> GetProductById(int id)
        => (await _productManager.GetProductByIdAsync(id)).MapToActionResult();

    [HttpGet]
    [Route($"{_controllerRoute}/getByBarcode")]
    public async Task<IActionResult> GetProductByBarcode(string barcode)
    {
        var user = GetUser();
        return (await _productManager.GetProductByBarcodeAsync(barcode, user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/suggest")]
    public async Task<IActionResult> SuggestProduct(string barcode)
        => (await _productManager.SuggestProductAsync(barcode)).MapToActionResult();

    [HttpGet]
    [Route($"{_controllerRoute}/getProductsWithNameLike")]
    public async Task<IActionResult> GetProductsWithNameLike(string name, int pageSize)
        => (await _productManager.GetProductsWithNameLike(name, pageSize)).MapToActionResult();

}