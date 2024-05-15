using Server.Data.Models;

namespace Server.ViewModels.Fridge;

public class AllFridgesViewModel
{
    public required FridgeViewModel[] Fridges { get; set; }
    public required FridgeViewModel[] SharedFridges { get; set; }
}