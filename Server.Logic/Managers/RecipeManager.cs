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
    private readonly IRecipeViewModelFactory _recipeViewModelFactory;
    private const string _recipeNotFound = "Recipe not found";
    private const string _categoryNotFound = "Category not found";
    private const string _commentNotFound = "Comment not found";
    private const string _userIsNotTheAuthor = "User are not the author of the recipe";
    private const string _userDoNotLikeRecipe = "User does not like selected recipe";

    public RecipeManager(GitfoodContext database, IPageingManager pageingManager,
     IPathProvider pathProvider, IFileSaver fileSaver, IRecipeViewModelFactory recipeViewModelFactory)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
        _pageingManager = pageingManager ?? throw new ArgumentNullException(nameof(pageingManager));
        _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
        _fileSaver = fileSaver ?? throw new ArgumentNullException(nameof(fileSaver));
        _recipeViewModelFactory = recipeViewModelFactory ?? throw new ArgumentNullException(nameof(recipeViewModelFactory));
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
	await _dbInfo.SaveChangesAsync();

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

    public async Task<IManagerActionResult<RecipeFullViewModel>> GetRecipeByIdAsync(int id, string user)
    {
        var recipe = await _dbInfo.Recipes
            .Include(x => x.RecipiesIngredients)
            .Include(x => x.RecipiesImages)
            .Include(x => x.Categories)
            .Include(x => x.Users)
            .Include(x => x.RecipesComments)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (recipe == null)
        {
            return new ManagerActionResult<RecipeFullViewModel>(null, ResultEnum.BadRequest, _recipeNotFound);
        }
        var result = _recipeViewModelFactory.CreateFullViewModel(recipe, user, (await GetMainImageAsync(id))?.ImagePath);

        return new ManagerActionResult<RecipeFullViewModel>(result, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> AddCommentAsync(int recipeId, string comment, string user)
        => await new DatabaseExceptionHandler().HandleExceptionsAsync(async () => await AddCommentInternalAsync(recipeId, comment, user));

    private async Task<IManagerActionResult> AddCommentInternalAsync(int recipeId, string comment, string user)
    {
        var newComment = new RecipesComment
        {
            Message = comment,
            Recipe = recipeId,
            User = user,
            Date = DateTime.Now
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
        => await new DatabaseExceptionHandler().HandleExceptionsAsync(async () => await LikeRecipeInternalAsync(recipeId, user));

    private async Task<IManagerActionResult> LikeRecipeInternalAsync(int recipeId, string user)
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
        if (recipe is null)
            return new ManagerActionResult(ResultEnum.BadRequest, _recipeNotFound);

        var userEntity = await _dbInfo.Users.FirstOrDefaultAsync(x => x.Login == user);
        if(userEntity is null)
            return new ManagerActionResult(ResultEnum.BadRequest, _userDoNotLikeRecipe);

        recipe.Users.Remove(userEntity);
        await _dbInfo.SaveChangesAsync();

        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<RecipesComment[]>> GetRecipeCommentsPagedAsync(int recipeId, int page, int pageSize)
    {
        IQueryable<RecipesComment> comments = _dbInfo.RecipesComments.Where(x => x.Recipe == recipeId).OrderByDescending(x => x.Date);
        return new ManagerActionResult<RecipesComment[]>(await _pageingManager.GetPagedInfo(comments, page, pageSize).ToArrayAsync(), ResultEnum.OK);
    }

    public async Task<IManagerActionResult<RecipeOutViewModel[]>> GetRecipesPagedAsync(int page, int pageSize, string searchName, int[] categoryIds, string user)
    {
        IQueryable<Recipe> data = _dbInfo.Recipes.Include(x => x.Categories);
        if(!searchName.IsNullOrEmpty())
            data = data.Where(x => x.Name.Contains(searchName));
        if(categoryIds is not null && categoryIds.Length > 0) 
        {
            data = data.Where(x => x.Categories.Any(x => categoryIds.Contains(x.Id)));
        }

        var pagedInfo = await _pageingManager.GetPagedInfo(data, page, pageSize).ToArrayAsync();
        var result = new List<RecipeOutViewModel>();
        foreach(var info in pagedInfo) 
        {
            var likeInfo = await GetNumOfLikesAndIsItLiked(info.Id, user);
            result.Add(_recipeViewModelFactory.CreateBasicViewModel(info, user, (await GetMainImageAsync(info.Id))?.ImagePath, likeInfo.isLiked, likeInfo.numOfLikes));
        }
        return new ManagerActionResult<RecipeOutViewModel[]>(result.ToArray(), ResultEnum.OK);
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
        var changed = await _dbInfo.Recipes.Where(x => x.Id == id && x.Author == user).ExecuteUpdateAsync(setter => setter.SetProperty(x => x.Name, name));
        if(changed == 0)
            return new ManagerActionResult(ResultEnum.BadRequest, _recipeNotFound);
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

        await _dbInfo.SaveChangesAsync();
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

    public async Task<IManagerActionResult<string>> AddOrUpdateMainPhoto(int recipeId, string user, Stream stream, string fileName)
    {
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId && x.Author == user);
        if(recipe is null) 
            return new ManagerActionResult<string>(_userIsNotTheAuthor ,ResultEnum.BadRequest);
        var mainImage = await GetMainImageAsync(recipeId);
        if(mainImage is not null)
            await DeleteMainImageAsync(recipeId);
        
        var path = _pathProvider.GetMainImagePath(recipeId, fileName);
        await _fileSaver.SaveFileAsync(path , stream);
        await _dbInfo.RecipiesImages.AddAsync(new RecipiesImage()
        {
            Recipe = recipeId,
            Name = fileName,
            ImagePath = path
        });
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult<string>(path, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<RecipeExtendedViewModel>> GetRecipeDetailsAsync(int recipeId, string user)
    {
        var recipe = await _dbInfo.Recipes
            .Include(x => x.RecipiesIngredients)
            .Include(x => x.RecipiesImages)
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.Id == recipeId);

        if (recipe == null)
            return new ManagerActionResult<RecipeExtendedViewModel>(null, ResultEnum.BadRequest, _recipeNotFound);
        
        var likeInfo = await GetNumOfLikesAndIsItLiked(recipeId, user);
        var result = _recipeViewModelFactory.CreateExtendedViewModel(recipe, user, (await GetMainImageAsync(recipeId))?.ImagePath, likeInfo.isLiked, likeInfo.numOfLikes);
        return new ManagerActionResult<RecipeExtendedViewModel>(result, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> UpdateRecipeCategoriesAsync(int recipeId, string user, int[] categoryIds)
        => await new DatabaseExceptionHandler().HandleExceptionsAsync(async () => await UpdateRecipeCategoriesInternalAsync(recipeId, user,categoryIds));

    private async Task<IManagerActionResult> UpdateRecipeCategoriesInternalAsync(int recipeId, string user, int[] categoryIds)
    {
        using var trans = await _dbInfo.Database.BeginTransactionAsync();
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId && x.Author == user);
        if (recipe is null)
            return new ManagerActionResult(ResultEnum.BadRequest, _recipeNotFound);

        var categories = await _dbInfo.FoodCategories.Where(x => categoryIds.Contains(x.Id)).ToArrayAsync();
        recipe.Categories.Clear();
        foreach (var category in categories)
            recipe.Categories.Add(item: category);
        await _dbInfo.SaveChangesAsync();
        await trans.CommitAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }

    private async Task<(int numOfLikes, bool isLiked)> GetNumOfLikesAndIsItLiked(int recipeId, string user)
    {
        var userEntity = await _dbInfo.Users.FirstOrDefaultAsync(x => x.Login == user);
        var isLiked = userEntity is null ? false : await _dbInfo.Recipes.AnyAsync(x => x.Id == recipeId && x.Users.Any(y => y.Login == user));
        var numOfLikes = await _dbInfo.Recipes.Where(x => x.Id == recipeId).SelectMany(x => x.Users).CountAsync();
        return (numOfLikes, isLiked);
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

    private async Task<RecipiesImage> GetMainImageAsync(int recipeId)
        => await _dbInfo.RecipiesImages.FirstOrDefaultAsync(
            x => x.Recipe == recipeId 
            && x.ImagePath.Contains(_pathProvider.GetMainImagePathPrefix(recipeId)));

    private async Task<int> DeleteMainImageAsync(int recipeId)
        => await _dbInfo.RecipiesImages.Where(
            x => x.Recipe == recipeId 
            && x.ImagePath.Contains(_pathProvider.GetMainImagePathPrefix(recipeId))
            ).ExecuteDeleteAsync();
}
