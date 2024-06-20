using Server.Data.Models;

namespace Server.ViewModels.Fridge;

public class FridgeProductViewModel{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public required string Barcode { get; set; }
    public int Id { get; set; }
    public double? Quantity { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public required string CategoryStatus { get; set; }
    public required string CategoryUnit { get; set; }
}