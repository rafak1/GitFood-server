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

    public ProductManager(GitfoodContext database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task<IManagerActionResult<int>> AddProductAsync(ProductViewModel product, string user) 
        => await new DatabaseExceptionHandler<int>().HandleExceptionsAsync(async () => await AddProductInternalAsync(product, user));
        
    private async Task<IManagerActionResult<int>> AddProductInternalAsync(ProductViewModel product, string user) 
    {
        var transaction = await _dbInfo.Database.BeginTransactionAsync();
        await _dbInfo.Products.AddAsync(new Product{
            Name = product.Name,
            Description = product.Description,
            Barcode = product.Barcode,
            Quantity = product.Quantity,
            User = user,
            Category = product.CategoryId
        });
        await _dbInfo.SaveChangesAsync();
        var id = await _dbInfo.Products.Where(x => x.Barcode == product.Barcode && x.User == user).Select(x => x.Id).FirstOrDefaultAsync();
        await transaction.CommitAsync();
        return new ManagerActionResult<int>(id, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<ProductWithCategoryViewModel>> GetProductByIdAsync(int id) 
    {
        var result = await _dbInfo.Products.Include(x => x.CategoryNavigation).FirstOrDefaultAsync(x => x.Id == id);
        if(result is null)
            return new ManagerActionResult<ProductWithCategoryViewModel>(null, ResultEnum.NotFound);
        return new ManagerActionResult<ProductWithCategoryViewModel>(GetProductAllInfo(result), ResultEnum.OK);
    }

    public async Task<IManagerActionResult> DeleteProductAsync(int id) 
    {
        await _dbInfo.Products.Where(x => x.Id == id).ExecuteDeleteAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<ProductWithCategoryViewModel>> GetProductByBarcodeAsync(string barcode, string user)
    {
        var result = await _dbInfo.Products.Where(x => x.Barcode == barcode && x.User == user).Include(x => x.CategoryNavigation).ToArrayAsync();
        if (result.Length == 0)
            return new ManagerActionResult<ProductWithCategoryViewModel>(null, ResultEnum.NotFound);
        return new ManagerActionResult<ProductWithCategoryViewModel>(GetProductAllInfo(result.FirstOrDefault()), ResultEnum.OK);
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
        var result = await _dbInfo.Products
            .Where(x => x.Barcode == barcode)
            .Include(x => x.CategoryNavigation)
            .ToListAsync();

        var product = result
            .GroupBy(x => x.Category)
            .OrderByDescending(x => x.Count())
            .FirstOrDefault();
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
                }
            },
            Category = new IdExtendedViewModel<CategoryViewModel>()
            {
                Id = product.CategoryNavigation.Id,
                InnerInformation = new CategoryViewModel()
                {
                    Name = product.CategoryNavigation.Name,
                    Unit = product.CategoryNavigation.Unit,
                    Status = product.CategoryNavigation.Status
                }
            }
        };
    }
}