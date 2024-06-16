namespace Server.ViewModels.Recipes;

public class RecipeSearchViewModel 
{
    public string? SearchName {get; set;}
    public int[]? IngredientsIds {get; set;}
    public int[]? FoodCategoriesIds {get; set;}
    public int[]? FridgeIds {get; set;}
}
