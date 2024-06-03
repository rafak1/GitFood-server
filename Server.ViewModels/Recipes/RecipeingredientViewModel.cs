namespace Server.ViewModels.Recipes;

public class RecipeIngredientViewModel
{
    public required int CategoryId { get; set; }
    public required String CategoryName { get; set; }
    public required String Unit { get; set; }
    public double? Quantity { get; set; }
}
