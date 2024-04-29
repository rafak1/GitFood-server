using Server.Logic.Abstract.Managers;
using Server.ViewModels.Products;
using Server.ViewModels;
using Server.Data.Models;
using Server.Database;
using Server.ViewModels.Categories;
using Server.ViewModels.Barcodes;
using Microsoft.EntityFrameworkCore;
using Server.Logic.Abstract;
using NuGet.Packaging;

namespace Server.Logic.Managers;

internal class ProductManager : IProductManager
{
    private readonly GitfoodContext _dbInfo;
    private const string ControllerPart = "products";

    public ProductManager(GitfoodContext database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task<IManagerActionResult> AddProductAsync(ProductViewModel product, string user) 
    {
        await _dbInfo.Products.AddAsync(new Product{
            Name = product.Name,
            Description = product.Description,
            Barcode = product.Barcode,
            Quantity = product.Quantity,
            User = user,
            Category = product.CategoryId
        });
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<ProductWithCategoryViewModel>> GetProductByIdAsync(int id) 
    {
        var result = await _dbInfo.Products.FirstOrDefaultAsync(x => x.Id == id);
        if(result is null)
            return new ManagerActionResult<ProductWithCategoryViewModel>(null, ResultEnum.NotFound);
        return new ManagerActionResult<ProductWithCategoryViewModel>(GetProductAllInfo(result), ResultEnum.OK);
    }

    public async Task<IManagerActionResult> DeleteProductAsync(int id) 
    {
        await _dbInfo.Products.Where(x => x.Id == id).ExecuteDeleteAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<ProductWithCategoryViewModel[]>> GetProductByBarcodeAsync(string barcode, string user)
    {
        var result = await _dbInfo.Products.Where(x => x.Barcode == barcode && x.User == user).ToArrayAsync();
        return new ManagerActionResult<ProductWithCategoryViewModel[]>(result.Select(GetProductAllInfo).ToArray(), ResultEnum.OK);
    }

    public async Task<IManagerActionResult> UpdateProductAsync(ProductViewModel product, string user, int id)
    {
        var existingProduct = await _dbInfo.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (existingProduct != null)
        {
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Quantity = product.Quantity;
            existingProduct.Category = product.CategoryId;

            await _dbInfo.SaveChangesAsync();
        }
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<ProductWithCategoryViewModel>> SuggestProductAsync(string barcode)
    {
        var product = await _dbInfo.Products
            .Where(x => x.Barcode == barcode)
            .GroupBy(x => x.Barcode)
            .OrderByDescending(x => x.Count())
            .FirstOrDefaultAsync();
        return new ManagerActionResult<ProductWithCategoryViewModel>(GetProductAllInfo(product.FirstOrDefault()), ResultEnum.OK);
    }

    private ProductWithCategoryViewModel GetProductAllInfo(Product product) 
    {
        return new ProductWithCategoryViewModel() 
        {
            Product = new IdExtendedViewModel<ProductViewModel>() 
            {
                Id = product.Id,
                InnerInformation = new ProductViewModel()
                {
                    Name = product.Name,
                    Description = product.Description,
                    Barcode = product.Barcode,
                    Quantity = (double)product.Quantity,
                    CategoryId = product.Category.Value,
                    User = product.User
                }
            },
            Category = new IdExtendedViewModel<CategoryViewModel>()
            {
                //TODO
            }
        };
    }
}