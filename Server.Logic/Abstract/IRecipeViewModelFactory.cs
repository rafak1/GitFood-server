using Server.ViewModels.Recipes;
using Server.Data.Models;

namespace Server.Logic.Abstract;

internal interface IRecipeViewModelFactory
{
    public RecipeOutViewModel CreateBasicViewModel(Recipe recipe, string user, string titleImagePath);
    public RecipeExtendedViewModel CreateExtendedViewModel(Recipe recipe, string user, string titleImagePath);
    public RecipeFullViewModel CreateFullViewModel(Recipe recipe, string user, string titleImagePath);
}