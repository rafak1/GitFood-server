using Server.ViewModels.Products;

namespace Server.Logic.Abstract.Managers;

public interface IProductManager
{
    public Task<IManagerActionResult<int>> AddProductAsync(ProductViewModel product, string user);
    public Task<IManagerActionResult> UpdateProductAsync(ProductViewModel product, string user, int id);
    public Task<IManagerActionResult<ProductWithCategoryViewModel>> GetProductByBarcodeAsync(string barcode, string user);
    public Task<IManagerActionResult<ProductWithCategoryViewModel>> GetProductByIdAsync(int id);
    public Task<IManagerActionResult> DeleteProductAsync(int id);
    public Task<IManagerActionResult<ProductWithCategoryViewModel>> SuggestProductAsync(string barcode);
    public Task<IManagerActionResult<ProductWithCategoryViewModel[]>> GetProductsWithNameLike(string name, int pageSize);
}