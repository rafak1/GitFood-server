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
using SQLitePCL;

namespace Server.Logic.Managers;

internal class ProductManager : IProductManager
{
    private readonly GitfoodContext _dbInfo;
    private const string ControllerPart = "products";

    public ProductManager(GitfoodContext database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task<IManagerActionResult> AddProductAsync(ProductViewModel product) 
    {
        await _dbInfo.Products.AddAsync(new Product{
            Name = product.Name,
            Description = product.Description
        });
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }
    public async Task<IManagerActionResult<ProductWithCategoryAndBarcodeViewModel[]>> GetProductsAsync(string name) 
    {
        var rawProducts = await _dbInfo.Products.Where(x => x.Name.Contains(name)).ToArrayAsync();
        var products = rawProducts.Select(GetProductAllInfo).ToArray();
        return new ManagerActionResult<ProductWithCategoryAndBarcodeViewModel[]>(products, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<ProductWithCategoryAndBarcodeViewModel>> GetProductByIdAsync(int id) 
    {
        var result = await _dbInfo.Products.FirstOrDefaultAsync(x => x.Id == id);
        if(result is null)
            return new ManagerActionResult<ProductWithCategoryAndBarcodeViewModel>(null, ResultEnum.NotFound);
        return new ManagerActionResult<ProductWithCategoryAndBarcodeViewModel>(GetProductAllInfo(result), ResultEnum.OK);
    }

    public async Task<IManagerActionResult> DeleteProductAsync(int id) 
    {
        await _dbInfo.Products.Where(x => x.Id == id).ExecuteDeleteAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }
    public async Task<IManagerActionResult> AddCategoriesToProductAsync(ProductToCategoriesViewModel model) 
    {
        var product = await _dbInfo.Products.FirstOrDefaultAsync(x => x.Id == model.ProductId);
        if(product is null || model.CategoriesIds is null)
            return new ManagerActionResult(ResultEnum.NotFound);
        _dbInfo.Products.Update(product);
        product.Categories.AddRange(_dbInfo.Categories.Where(x => model.CategoriesIds.Contains(x.Id)));
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
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

    public async Task<IManagerActionResult> AddProductWithBarcodeAsync(ProductWithBarcodeViewModel productBarcodeViewModel, string user)
    {
        /*
            Problemy:
                 -> brak transakcyjności, 
                 -> productWithId będzie miało id pierwszego produktu o takiej nazwie i opisie, a nie tego, który dodaliśmy
                 -> brzydkie
            Możliwe rozwiązanie:
                 -> Jedno wielkie zapytanie SQL
        */
        await _dbInfo.Products.AddAsync(new Product{
            Name = productBarcodeViewModel.Name,
            Description = productBarcodeViewModel.Description
        });
        await _dbInfo.SaveChangesAsync();
        var productWithId = await _dbInfo.Products.FirstOrDefaultAsync(x => x.Name == productBarcodeViewModel.Name && x.Description == productBarcodeViewModel.Description);
        if(productWithId is null)
            return new ManagerActionResult(ResultEnum.BadRequest);
        await _dbInfo.Barcodes.AddAsync(new Barcode{
            Key = productBarcodeViewModel.Barcode,
            ProductId = productWithId.Id,
            User = user
        });
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<ProductWithCategoryAndBarcodeViewModel>> GetProductByBarcodeAsync(string barcode, string user)
    {
        var dbBarcode = await _dbInfo.Barcodes.FirstOrDefaultAsync(x => x.Key == barcode && x.User == user);
        if(dbBarcode is null)
            return new ManagerActionResult<ProductWithCategoryAndBarcodeViewModel>(null, ResultEnum.NotFound);

        var product = await _dbInfo.Products.FirstOrDefaultAsync(x => x.Id == dbBarcode.ProductId);
        if(product is null)
            return new ManagerActionResult<ProductWithCategoryAndBarcodeViewModel>(null, ResultEnum.NotFound);
        return new ManagerActionResult<ProductWithCategoryAndBarcodeViewModel>(GetProductAllInfo(product), ResultEnum.OK);


    }
}