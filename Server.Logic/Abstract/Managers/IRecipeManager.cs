using Server.Data.Models;
using Server.ViewModels.Recipes;

namespace Server.Logic.Abstract.Managers;

public interface IRecipeManager
{
    public Task<IManagerActionResult<int>> CreateRecipeAsync(RecipeViewModel recipe, string user);
    public Task<IManagerActionResult> DeleteRecipeAsync(int id, string user);
    public Task<IManagerActionResult<Recipe>> GetRecipeByIdAsync(int id, string user);
    public Task<IManagerActionResult> AddCommentAsync(int recipeId, string comment, string user);
    public Task<IManagerActionResult> RemoveCommentAsync(int commentId, string user);
    public Task<IManagerActionResult> LikeRecipeAsync(int recipeId, string user);
    public Task<IManagerActionResult> UnlikeRecipeAsync(int recipeId, string user);
    public Task<IManagerActionResult<Recipe[]>> GetRecipesPagedAsync(int page, int pageSize, string searchName, int[] categoryIds);
}