using Server.Data.Models;

namespace Server.ViewModels.Recipes;

public class RecipeIngredientExtendedViewModel
{
    public required int CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public double? Quantity { get; set; }
    public string? Units { get; set; }
}
