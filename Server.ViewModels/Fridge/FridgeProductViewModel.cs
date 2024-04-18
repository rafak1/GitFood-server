namespace Server.ViewModels.Fridge;

public class FridgeProductViewModel 
{
    public int ProductId { get; set; }
    
    public required string Login { get; set; }

    public double Quantity { get; set; }

    public required string Unit { get; set; }
}