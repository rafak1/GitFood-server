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
    private readonly IPathProvider _pathProvider;
    private readonly IFileSaver _fileSaver;
    private static readonly string _recipeNotFound = "Recipe not found";

    private static readonly string _categoryNotFound = "Category not found";

    private static readonly string _commentNotFound = "Comment not found";

    public RecipeManager(GitfoodContext database, IPageingManager pageingManager,
     IPathProvider pathProvider, IFileSaver fileSaver)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
        _pageingManager = pageingManager ?? throw new ArgumentNullException(nameof(pageingManager));
        _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
        _fileSaver = fileSaver ?? throw new ArgumentNullException(nameof(fileSaver));
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

        newRecipe.MarkdownPath = await SaveMarkdownAsync(id, recipe.Markdown);

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

        //Ingredients
        if(recipe.Ingredients != null && recipe.Ingredients.Count > 0)
        {
            await AddIngredientsAsync(newRecipe, recipe);
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

        //TODO: Usuniecie powiazanych plikow (MD + zdj)

        _dbInfo.Recipes.Remove(recipe);
        await _dbInfo.SaveChangesAsync();

        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<RecipeOutViewModel>> GetRecipeByIdAsync(int id, string user)
    {
        var recipe = await _dbInfo.Recipes
            .Include(x => x.RecipiesIngredients)
            .Include(x => x.RecipiesImages)
            .Include(x => x.Categories)
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (recipe == null)
        {
            return new ManagerActionResult<RecipeOutViewModel>(null, ResultEnum.BadRequest, _recipeNotFound);
        }

        return new ManagerActionResult<RecipeOutViewModel>(GetRecipeViewModel(recipe), ResultEnum.OK);
    }

    public async Task<IManagerActionResult> AddCommentAsync(int recipeId, string comment, string user)
    {
        var newComment = new RecipesComment
        {
            Message = comment,
            Recipe = recipeId,
            User = user,
            Date = DateTimeOffset.Now;
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
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId && x.Author == user);
        if (recipe == null)
        {
            return new ManagerActionResult(ResultEnum.BadRequest, _recipeNotFound);
        }

        recipe.Users.Remove(await _dbInfo.Users.FirstOrDefaultAsync(x => x.Login == user));
        await _dbInfo.SaveChangesAsync();

        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<RecipesComment[]>> GetRecipeCommentsPagedAsync(int recipeId, int page, int pageSize)
    {
        IQueryable<RecipesComment> comments = _dbInfo.RecipesComments.Where(x => x.Recipe == recipeId);
        return new ManagerActionResult<RecipesComment[]>(await _pageingManager.GetPagedInfo(comments, page, pageSize).ToArrayAsync(), ResultEnum.OK);
    }

    public async Task<IManagerActionResult<RecipeOutViewModel[]>> GetRecipesPagedAsync(int page, int pageSize, string searchName, int[] categoryIds)
    {
        IQueryable<Recipe> data = _dbInfo.Recipes.Include(x => x.Categories).Include(x => x.Users);
        if(!searchName.IsNullOrEmpty())
            data = data.Where(x => x.Name.Contains(searchName));
        if(categoryIds is not null && categoryIds.Length > 0) 
        {
            data = data.Where(x => x.Categories.Any(x => categoryIds.Contains(x.Id)));
        }
        IQueryable<RecipeOutViewModel> recipes = data.Select(x => GetRecipeViewModel(x));
        return new ManagerActionResult<RecipeOutViewModel[]>(await _pageingManager.GetPagedInfo(recipes, page, pageSize).ToArrayAsync(),ResultEnum.OK);
    }

    public async Task<IManagerActionResult> AddReferenceToRecipeAsync(int id, int referenceId, double multiplayer, string user)
    {
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == id && x.Author == user);
        if (recipe == null)
            return new ManagerActionResult(ResultEnum.BadRequest, _recipeNotFound);
        await _dbInfo.RecipeChildren.AddAsync(
            new RecipeChild()
            {
                Recipe = id,
                Child = referenceId,
                Multiplier = multiplayer
            }
        );
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult> RemoveReferenceToRecipeAsync(int id, int referenceId, string user)
    {
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == id && x.Author == user);
        if (recipe == null)
            return new ManagerActionResult(ResultEnum.BadRequest, _recipeNotFound);
        await _dbInfo.RecipeChildren.Where(x => x.Recipe == id && x.Child == referenceId).ExecuteDeleteAsync();
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult> UpdateDescriptionAsync(int id, string description, string user)
    {
        await _dbInfo.Recipes.Where(x => x.Id == id && x.Author == user).ExecuteUpdateAsync(setter => setter.SetProperty(x => x.Description, description));
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult> UpdateRecipeNameAsync(int id, string name, string user)
    {
        await _dbInfo.Recipes.Where(x => x.Id == id && x.Author == user).ExecuteUpdateAsync(setter => setter.SetProperty(x => x.Name, name));
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    private async Task<IManagerActionResult> DeleteImageAsync(int recipeId, string imageName)
    {
        await _dbInfo.RecipiesImages.Where(x => x.Recipe == recipeId && x.Name == imageName).ExecuteDeleteAsync();
        await _dbInfo.SaveChangesAsync();
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId);
        var imagePath = _pathProvider.GetImagePath(recipeId, imageName);

        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult> UpdateIngredientsAsync(int recipeId, int categoryId, double quantity, string user) 
    {
        var changed = await _dbInfo.RecipiesIngredients.Where(x => x.Reciepie == recipeId && x.Category == categoryId && x.ReciepieNavigation.Author == user).ExecuteUpdateAsync(setter => setter.SetProperty(x => x.Quantity, quantity));
        await _dbInfo.SaveChangesAsync();
        if(changed == 0) 
        {
            await _dbInfo.RecipiesIngredients.AddAsync(new RecipiesIngredient
            {
                Reciepie = recipeId,
                Category = categoryId,
                Quantity = quantity
            });
            await _dbInfo.SaveChangesAsync();
        }
        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<string[]>> AddImagesAsync(int recipeId, RecipeImageViewModel[] images, string user)
    {
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId && x.Author == user);
        if(recipe is null)
            return new ManagerActionResult<string[]>(null, ResultEnum.BadRequest);
        using var transaction = await _dbInfo.Database.BeginTransactionAsync();
        var imagesUri = new List<string>();
        foreach (var image in images)
        {
            imagesUri.Add(await AddAndSaveImageAsync(recipe, image));
        }

        await transaction.CommitAsync();
        return new ManagerActionResult<string[]>(imagesUri.ToArray(), ResultEnum.OK);
    }

    public async Task<IManagerActionResult> DeleteImagesAsync(int recipeId, string[] imageNames, string user)
    {
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId && x.Author == user);
        if(recipe is null) 
            return new ManagerActionResult(ResultEnum.BadRequest);
        foreach(var imageName in imageNames)
            await DeleteImageAsync(recipeId, imageName);
        return new ManagerActionResult(ResultEnum.OK);

    }

    public async Task<IManagerActionResult> UpdateMarkdownAsync(int recipeId, string markdown, string user)
    {
        var recipie = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId && x.Author == user);
        if(recipie is null)
            return new ManagerActionResult(ResultEnum.BadRequest);
        await SaveMarkdownAsync(recipeId, markdown);
        return new ManagerActionResult(ResultEnum.OK);
    }

    private async Task<string> SaveMarkdownAsync(int recipeId, string markdown)
    {
        var markdownPath = _pathProvider.GetMarkdownPath(recipeId);
        using var streamToSave = new MemoryStream(Encoding.UTF8.GetBytes(markdown ?? ""));
        await _fileSaver.SaveFileAsync(markdownPath, streamToSave);
        return markdownPath;
    }

    private async Task AddIngredientsAsync(Recipe recipe, RecipeViewModel recipeViewModel)
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

    private async Task<string> AddAndSaveImageAsync(Recipe recipe, RecipeImageViewModel image)
    {
        var imagePath = _pathProvider.GetImagePath(recipe.Id, image.Name);
        await _fileSaver.SaveFileAsync(imagePath, image.Image);

        var newImage = new RecipiesImage
        {
            Name = image.Name,
            ImagePath = imagePath,
            Recipe = recipe.Id
        };

        await _dbInfo.RecipiesImages.AddAsync(newImage);
        recipe.RecipiesImages.Add(newImage);
        return imagePath;
    }

    private static RecipeOutViewModel GetRecipeViewModel(Recipe recipe)
    {
        return new RecipeOutViewModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Author = recipe.Author,
            MarkdownPath = recipe.MarkdownPath,
            Ingredients = recipe.RecipiesIngredients.Select(x => new RecipeIngredientViewModel
            {
                CategoryId = x.Category,
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
            ImagePaths = recipe.RecipiesImages.Select(x => x.ImagePath).ToList()
        };
    }
}
