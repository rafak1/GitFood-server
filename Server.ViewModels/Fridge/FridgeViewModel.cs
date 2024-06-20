namespace Server.ViewModels.Fridge;

public class FridgeViewModel{
    public int FridgeId { get; set; }
    public string? Owner { get; set; }
    public required List<FridgeProductViewModel> Products { get; set; }
    public required string Name { get; set; }
    public required string[] SharedWith { get; set; }
} 