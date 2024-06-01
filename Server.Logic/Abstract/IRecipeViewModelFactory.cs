using Server.ViewModels.Recipes;
using Server.Data.Models;

namespace Server.Logic.Abstract;

internal interface IRecipeViewModelFactory
{
    public RecipeOutViewModel CreateBasicViewModel(Recipe recipe, string user, string titleImagePath, bool isLiked, int numOfLikes);
    public RecipeExtendedViewModel CreateExtendedViewModel(Recipe recipe, string user, string titleImagePath, bool isLiked, int numOfLikes);
    public RecipeFullViewModel CreateFullViewModel(Recipe recipe, string user, string titleImagePath);
}