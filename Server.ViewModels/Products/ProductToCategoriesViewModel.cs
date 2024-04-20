namespace Server.ViewModels.Products;

public class ProductToCategoriesViewModel
{
    public int ProductId {get; set;}
    public required int[] CategoriesIds {get; set;}
}