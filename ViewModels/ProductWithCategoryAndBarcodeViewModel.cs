namespace Server.ViewModels;

public class ProductWithCategoryAndBarcodeViewModel 
{
    public IdExtendedViewModel<ProductViewModel> Product {get; set;}
    public BarcodeViewModel[] Barcodes {get; set;}
    public IdExtendedViewModel<CategoryViewModel>[] Categories {get; set;}
}