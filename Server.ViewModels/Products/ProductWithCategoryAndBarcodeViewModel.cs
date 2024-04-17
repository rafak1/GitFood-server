using Server.ViewModels.Barcodes;
using Server.ViewModels.Categories;

namespace Server.ViewModels.Products;

public class ProductWithCategoryAndBarcodeViewModel 
{
    public required IdExtendedViewModel<ProductViewModel> Product {get; set;}
    public required BarcodeViewModel[] Barcodes {get; set;}
    public required IdExtendedViewModel<CategoryViewModel>[] Categories {get; set;}
}