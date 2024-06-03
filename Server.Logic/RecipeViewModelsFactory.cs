using Server.Data.Models;
using Server.Logic.Abstract;
using Server.ViewModels.Recipes;

namespace Server.Logic;

public class RecipeViewModelFactory : IRecipeViewModelFactory
{
    public RecipeOutViewModel CreateBasicViewModel(Recipe recipe, string user, string titleImagePath, bool isLiked, int numOfLikes)
    {
        return new RecipeOutViewModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Author = recipe.Author,
            MarkdownPath = recipe.MarkdownPath,
            Categories = recipe.Categories.Select(x => x.Id).ToList(),
            NumberOfLikes = numOfLikes,
            IsLiked = isLiked,
            TitleImage = titleImagePath,
        };
    }

    public RecipeExtendedViewModel CreateExtendedViewModel(Recipe recipe, string user, string titleImagePath, bool isLiked, int numOfLikes)
    {
        return new RecipeExtendedViewModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Author = recipe.Author,
            MarkdownPath = recipe.MarkdownPath,
            Ingredients = recipe.RecipiesIngredients.Select(x => new RecipeIngredientViewModel
            {
                CategoryId = x.Category,
		CategoryName = x.CategoryNavigation.Name,
                Quantity = x.Quantity
            }).ToList(),
            Categories = recipe.Categories.Select(x => x.Id).ToList(),
            ImagePaths = recipe.RecipiesImages.Select(x => x.ImagePath).ToList(),
            NumberOfLikes = numOfLikes,
            IsLiked = isLiked,
            TitleImage = titleImagePath,
        };
    }

    public RecipeFullViewModel CreateFullViewModel(Recipe recipe, string user, string titleImagePath)
    {
        return new RecipeFullViewModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Author = recipe.Author,
            MarkdownPath = recipe.MarkdownPath,
            Ingredients = recipe.RecipiesIngredients.Select(x => new RecipeIngredientViewModel
            {
                CategoryId = x.Category,
		CategoryName = x.CategoryNavigation.Name,
                Quantity = x.Quantity
            }).ToList(),
            Categories = recipe.Categories.Select(x => x.Id).ToList(),
            Likes = recipe.Users.Select(x => x.Login).ToList(),
            Comments = recipe.RecipesComments.Select(x => new RecipeCommentViewModel
            {
                Id = x.Id,
                Message = x.Message,
                Author = x.User,
                Likes = x.Likes,
                Date = x.Date
            }).ToList(),
            ImagePaths = recipe.RecipiesImages.Select(x => x.ImagePath).ToList(),
            NumberOfLikes = recipe.Users.Count,
            IsLiked = recipe.Users.Any(x => x.Login == user),
            TitleImage = titleImagePath
        };
    }
}
