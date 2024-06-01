namespace Server.ViewModels.Recipes;

public class RecipeFullViewModel : RecipeExtendedViewModel
{
    public List<string>? Likes { get; set; }
    public List<RecipeCommentViewModel>? Comments { get; set; }
}