namespace Server.ViewModels.Products;

public class ProductViewModel
{
    public required string Description { get; set; }
    public required string Name { get; set; }
    public required string Barcode { get; set; }
    public int CategoryId { get; set; }
    public double Quantity { get; set; }
}