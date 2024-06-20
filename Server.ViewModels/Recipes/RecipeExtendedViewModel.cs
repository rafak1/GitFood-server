namespace Server.ViewModels.Recipes;

public class RecipeExtendedViewModel : RecipeOutViewModel
{
    public List<RecipeIngredientViewModel>? Ingredients { get; set; }
    public List<string>? ImagePaths { get; set; }
}