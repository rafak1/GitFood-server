namespace Server.ViewModels.Fridge;

public class FridgeMapEntryViewModel
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required string Owner { get; set; }

    public string[]? SharedWith { get; set; }

    public required bool isShared { get; set; }
}