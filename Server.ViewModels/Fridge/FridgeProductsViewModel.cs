namespace Server.ViewModels.Fridge;

public class FridgeProductsViewModel 
{
    public required FridgeAddProductViewModel[] Products;
    public required int FridgeId;
}


public class FridgeAddProductViewModel
{
    public required int ProductId { get; set; }
    public required int Quantity { get; set; }
}