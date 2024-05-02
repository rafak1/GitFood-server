namespace Server.ViewModels.Products;


public class RecipeViewModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Markdown { get; set; }
    public List<RecipeIngredientViewModel>? Ingredients { get; set; }
    public List<int>? Categories { get; set; }
    public List<RecipeImageViewModel>? Images { get; set; }
}