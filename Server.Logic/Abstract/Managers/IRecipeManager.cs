using Server.Data.Models;
using Server.ViewModels.Recipes;

namespace Server.Logic.Abstract.Managers;

public interface IRecipeManager
{
    public Task<IManagerActionResult<int>> CreateRecipeAsync(RecipeViewModel recipe, string user);
    public Task<IManagerActionResult<string[]>> AddImagesAsync(int recipeId, RecipeImageViewModel[] images, string user);
    public Task<IManagerActionResult> DeleteRecipeAsync(int id, string user);
    public Task<IManagerActionResult<Recipe>> GetRecipeByIdAsync(int id, string user);
    public Task<IManagerActionResult> AddCommentAsync(int recipeId, string comment, string user);
    public Task<IManagerActionResult> RemoveCommentAsync(int commentId, string user);
    public Task<IManagerActionResult> LikeRecipeAsync(int recipeId, string user);
    public Task<IManagerActionResult> UnlikeRecipeAsync(int recipeId, string user);
    public Task<IManagerActionResult> UpdateMarkdownAsync(int recipeId, string markdown, string user);
    public Task<IManagerActionResult<Recipe[]>> GetRecipesPagedAsync(int page, int pageSize, string searchName, int[] categoryIds);
    public Task<IManagerActionResult> DeleteImagesAsync(int recipeId, string[] imageNames, string user);
    public Task<IManagerActionResult> UpdateIngredientsAsync(int recipeId, int categoryId, double quantity, string user);
    public Task<IManagerActionResult> AddReferenceToRecipeAsync(int id, int referenceId, double multiplayer, string user);
    public Task<IManagerActionResult> RemoveReferenceToRecipeAsync(int id, int referenceId, string user);
    public Task<IManagerActionResult> UpdateDescriptionAsync(int id, string description, string user);
    public Task<IManagerActionResult> UpdateRecipeNameAsync(int id, string name, string user);
}