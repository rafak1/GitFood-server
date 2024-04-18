using Server.Logic.Abstract.Managers;
using Server.ViewModels.Products;
using Server.ViewModels;
using Server.Data.Models;
using Server.Database;
using Server.ViewModels.Categories;
using Server.ViewModels.Barcodes;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace Server.Logic.Managers;

public class ProductManager : IProductManager
{
    private readonly GitfoodContext _dbInfo;
    private const string ControllerPart = "products";

    public ProductManager(GitfoodContext database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task AddProductAsync(ProductViewModel product) 
    {
        await _dbInfo.Products.AddAsync(new Product{
            Name = product.Name,
            Description = product.Description
        });
        await _dbInfo.SaveChangesAsync();
    }
    public async Task<ProductWithCategoryAndBarcodeViewModel[]> GetProductsAsync(string name) 
    {
        var result = await _dbInfo.Products.Where(x => x.Name.Contains(name)).ToArrayAsync();
        return result.Select(GetProductAllInfo).ToArray();
    }

    public async Task<ProductWithCategoryAndBarcodeViewModel> GetProductByIdAsync(int id) 
    {
        var result = await _dbInfo.Products.FirstOrDefaultAsync(x => x.Id == id);
        if(result is null)
            return null;
        return GetProductAllInfo(result);
    }

    public async Task DeleteProductAsync(int id) 
    {
        await _dbInfo.Products.Where(x => x.Id == id).ExecuteDeleteAsync();
    }
    public async Task AddCategoriesToProductAsync(ProductToCategoriesViewModel model) 
    {
        var product = await _dbInfo.Products.FirstOrDefaultAsync(x => x.Id == model.ProductId);
        if(product is null || model.CategoriesIds is null)
            return;
        _dbInfo.Products.Update(product);
        product.Categories.AddRange(_dbInfo.Categories.Where(x => model.CategoriesIds.Contains(x.Id)));
        await _dbInfo.SaveChangesAsync();
    }

    private ProductWithCategoryAndBarcodeViewModel GetProductAllInfo(Product product) 
    {
        return new ProductWithCategoryAndBarcodeViewModel() 
        {
            Product = new IdExtendedViewModel<ProductViewModel>() 
            {
                Id = product.Id,
                InnerInformation = new ProductViewModel()
                {
                    Name = product.Name,
                    Description = product.Description
                }
            },
            Categories = product.Categories.Select(x => 
                new IdExtendedViewModel<CategoryViewModel>() 
                {
                    Id = x.Id,
                    InnerInformation = new CategoryViewModel() 
                    {
                        Name = x.Name
                    }
                }
            ).ToArray(),
            Barcodes = product.Barcodes.Select(x => 
                new BarcodeViewModel() 
                {
                    BarcodeNumber = x.Key,
                    ProductId = x.ProductId.Value
                }
            ).ToArray()
        };
    }
}