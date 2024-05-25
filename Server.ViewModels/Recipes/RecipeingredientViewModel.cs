namespace Server.ViewModels.Recipes;

public class RecipeIngredientViewModel
{
    public required int CategoryId { get; set; }
    public double? Quantity { get; set; }
}