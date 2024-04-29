namespace Server.ViewModels.Products;

public class ProductWithCategoryViewModel
{
    public required IdExtendedViewModel<ProductViewModel> Product { get; set; }
    public required IdExtendedViewModel<CategoryViewModel> Category { get; set; }
}