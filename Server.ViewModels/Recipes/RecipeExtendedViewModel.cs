namespace Server.ViewModels.Recipes;

public class RecipeExtendedViewModel : RecipeOutViewModel
{
    public List<RecipeIngredientExtendedViewModel>? Ingredients { get; set; }
    public List<string>? ImagePaths { get; set; }
}