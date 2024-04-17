using Server.ViewModels.Products;
using Server.ViewModels;

namespace Server.Logic.Abstract;

public interface IProductManager
{
    public Task AddProductAsync(ProductViewModel product);
    public Task<ProductWithCategoryAndBarcodeViewModel[]> GetProductsAsync(string name);
    public Task<ProductWithCategoryAndBarcodeViewModel> GetProductByIdAsync(int id);
    public Task DeleteProductAsync(int id);
    public Task AddCategoriesToProductAsync(ProductToCategoriesViewModel model);
}