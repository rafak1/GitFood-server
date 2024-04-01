using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Server.DataModel;
using Server.ViewModels;

namespace Server.Controllers;

[Authorize]
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
        var result = _dbInfo.Products.Where(x => x.Name.Contains(name));
        return Ok(GetProductsAllInfo(result));
    }

    [HttpGet]
    [Route($"{ControllerPart}/getById")]
    public IActionResult GetProductById(int id) 
    {
        return Ok(GetProductsAllInfo(_dbInfo.Products.Where(x => x.Id == id)));
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
        product.Categories.AddRange(_dbInfo.Categories.Where(x => model.CategoriesIds.Contains(x.Id)));
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

    private IQueryable<ProductWithCategoryAndBarcodeViewModel> GetProductsAllInfo(IQueryable<Product> products) 
    {
        return products.Select(x => new ProductWithCategoryAndBarcodeViewModel() {
            Product = new IdExtendedViewModel<ProductViewModel>() {
                Id = x.Id,
                InnerInformation = new ProductViewModel() {
                    Name = x.Name,
                    Description = x.Description
                }
            },
            Categories = x.Categories.Select(x => 
                new IdExtendedViewModel<CategoryViewModel>() {
                    Id = x.Id,
                    InnerInformation = new CategoryViewModel() {
                        Name = x.Name
                    }
                }
            ).ToArray(),
            Barcodes = x.Barcodes.Select(x => 
                new BarcodeViewModel() {
                    BarcodeNumber = x.Key,
                    ProductId = x.ProductId.Value
                }
            ).ToArray()
        });
    }

}
