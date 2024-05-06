using Server.Logic.Abstract.Managers;
using Server.ViewModels.Recipes;
using Server.Database;
using Server.Logic.Abstract;
using System.Text;
using Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Server.Logic.Managers;

internal class RecipeManager : IRecipeManager
{
    private readonly GitfoodContext _dbInfo;
    private readonly IPageingManager _pageingManager;
    private static readonly string _recipeNotFound = "Recipe not found";

    private static readonly string _categoryNotFound = "Category not found";

    private static readonly string _commentNotFound = "Comment not found";

    public RecipeManager(GitfoodContext database, IPageingManager pageingManager)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
        _pageingManager = pageingManager ?? throw new ArgumentNullException(nameof(pageingManager));
    }

    public async Task<IManagerActionResult<int>> CreateRecipeAsync(RecipeViewModel recipe, string user)
    {
        var transaction = await _dbInfo.Database.BeginTransactionAsync();

        var newRecipe =new Recipe
        {
            Name = recipe.Name,
            Description = recipe.Description,
            Author = user,
            MarkdownPath = ""
        };

        await _dbInfo.Recipes.AddAsync(newRecipe);
        await _dbInfo.SaveChangesAsync();
        var id = newRecipe.Id;

        var markdownPath = $"./recipes/{user}/{recipe.Name}_{id}.md";
        //TODO File.WriteAllBytes(markdownPath, Encoding.UTF8.GetBytes(recipe.Markdown));

        newRecipe.MarkdownPath = markdownPath;

        await _dbInfo.SaveChangesAsync();

        //Categories

        if (recipe.Categories != null)
        {
            foreach (var category in recipe.Categories)
            {
                var categoryEntity = await _dbInfo.FoodCategories.FirstOrDefaultAsync(x => x.Id == category);
                if (categoryEntity == null)
                {
                    await transaction.RollbackAsync();
                    return new ManagerActionResult<int>(-1, ResultEnum.BadRequest, _categoryNotFound);
                }

                newRecipe.Categories.Add(categoryEntity);
            }
        }

        await _dbInfo.SaveChangesAsync();

        //Images

        if (recipe.Images != null)
        {
            AddAndSaveImages(newRecipe, recipe);
        }

        //Ingredients
        if(recipe.Ingredients != null)
        {
            AddIngredients(newRecipe, recipe);
        }

        await transaction.CommitAsync();

        return new ManagerActionResult<int>(id, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> DeleteRecipeAsync(int id, string user)
    {
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == id && x.Author == user);
        if (recipe == null)
        {
            return new ManagerActionResult(ResultEnum.BadRequest, _recipeNotFound);
        }

        _dbInfo.Recipes.Remove(recipe);
        await _dbInfo.SaveChangesAsync();

        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<Recipe>> GetRecipeByIdAsync(int id, string user)
    {
        var recipe = await _dbInfo.Recipes
            .Include(x => x.RecipiesIngredients)
            .Include(x => x.RecipiesImages)
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.Id == id && x.Author == user);

        if (recipe == null)
        {
            return new ManagerActionResult<Recipe>(null, ResultEnum.BadRequest, _recipeNotFound);
        }

        return new ManagerActionResult<Recipe>(recipe, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> AddCommentAsync(int recipeId, string comment, string user)
    {
        var newComment = new RecipesComment
        {
            Message = comment,
            Recipe = recipeId,
            User = user
        };
        await _dbInfo.RecipesComments.AddAsync(newComment);
        await _dbInfo.SaveChangesAsync();

        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult> RemoveCommentAsync(int commentId, string user)
    {
        var comment = await _dbInfo.RecipesComments.FirstOrDefaultAsync(x => x.Id == commentId && x.User == user);
        if (comment == null)
        {
            return new ManagerActionResult(ResultEnum.BadRequest, _commentNotFound);
        }

        _dbInfo.RecipesComments.Remove(comment);
        await _dbInfo.SaveChangesAsync();

        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult> LikeRecipeAsync(int recipeId, string user)
    {
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId);
        if (recipe == null)
        {
            return new ManagerActionResult(ResultEnum.BadRequest, _recipeNotFound);
        }
        recipe.Users.Add(await _dbInfo.Users.FirstOrDefaultAsync(x => x.Login == user));
        await _dbInfo.SaveChangesAsync();

        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult> UnlikeRecipeAsync(int recipeId, string user)
    {
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId);
        if (recipe == null)
        {
            return new ManagerActionResult(ResultEnum.BadRequest, _recipeNotFound);
        }

        recipe.Users.Remove(await _dbInfo.Users.FirstOrDefaultAsync(x => x.Login == user));
        await _dbInfo.SaveChangesAsync();

        return new ManagerActionResult(ResultEnum.OK);
    }

    private async void AddAndSaveImages(Recipe recipe, RecipeViewModel recipeViewModel)
    {
        foreach (var image in recipeViewModel.Images)
        {
            var imagePath = $"./recipes/{recipe.Author}/{recipe.Name}_{recipe.Id}_{image.Name}.png";
            //TODO File.WriteAllBytes(imagePath, image.Data);

            var newImage = new RecipiesImage
            {
                Name = image.Name,
                ImagePath = imagePath,
                Recipe = recipe.Id
            };

            await _dbInfo.RecipiesImages.AddAsync(newImage);
            recipe.RecipiesImages.Add(newImage);
        }
    }


    private async void AddIngredients(Recipe recipe, RecipeViewModel recipeViewModel)
    {
        foreach (var ingredient in recipeViewModel.Ingredients)
        {
            var category = await _dbInfo.FoodCategories.FirstOrDefaultAsync(x => x.Id == ingredient.CategoryId);
            if (category == null)
                continue;

            var newIngredient = new RecipiesIngredient
            {
                Category = ingredient.CategoryId,
                Quantity = ingredient.Quantity,
                Reciepie = recipe.Id
            };

            await _dbInfo.RecipiesIngredients.AddAsync(newIngredient);
            recipe.RecipiesIngredients.Add(newIngredient);
        }
    }

    public async Task<IManagerActionResult<Recipe[]>> GetRecipesPagedAsync(int page, int pageSize, string searchName, int[] categoryIds)
    {
        IQueryable<Recipe> data = _dbInfo.Recipes;
        if(!searchName.IsNullOrEmpty())
            data = data.Where(x => x.Name.Contains(searchName));
        if(categoryIds is not null && categoryIds.Length > 0) 
        {
            var categoryIdsContainer = categoryIds.ToHashSet();
            data = data.Where(x => x.Categories.Any(y => categoryIdsContainer.Contains(y.Id)));
        }
        return new ManagerActionResult<Recipe[]>(await _pageingManager.GetPagedInfo(data, page, pageSize).ToArrayAsync(),ResultEnum.OK);
    }
}
