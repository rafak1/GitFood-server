using Server.ViewModels.Products;

namespace Server.Logic.Abstract.Managers;

public interface IProductManager
{
    public Task<IManagerActionResult> AddProductAsync(ProductViewModel product);
    public Task<IManagerActionResult<ProductWithCategoryAndBarcodeViewModel[]>> GetProductsAsync(string name);
    public Task<IManagerActionResult<ProductWithCategoryAndBarcodeViewModel>> GetProductByIdAsync(int id);
    public Task<IManagerActionResult> DeleteProductAsync(int id);
    public Task<IManagerActionResult> AddCategoriesToProductAsync(ProductToCategoriesViewModel model);
    public Task<IManagerActionResult> AddProductWithBarcodeAsync(ProductWithBarcodeViewModel product, string user);
    public Task<IManagerActionResult<ProductWithCategoryAndBarcodeViewModel>> GetProductByBarcodeAsync(string barcode, string user);
}