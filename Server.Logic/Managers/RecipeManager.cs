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
    private readonly IFileProvider _fileProvider;
    private const string _recipeNotFound = "Recipe not found";
    private const string _categoryNotFound = "Category not found";
    private const string _commentNotFound = "Comment not found";
    private const string _userIsNotTheAuthor = "User are not the author of the recipe";
    private const string _userDoNotLikeRecipe = "User does not like selected recipe";

    public RecipeManager(GitfoodContext database, IPageingManager pageingManager,
     IPathProvider pathProvider, IFileSaver fileSaver,
     IRecipeViewModelFactory recipeViewModelFactory, IFileProvider fileProvider)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
        _pageingManager = pageingManager ?? throw new ArgumentNullException(nameof(pageingManager));
        _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
        _fileSaver = fileSaver ?? throw new ArgumentNullException(nameof(fileSaver));
        _recipeViewModelFactory = recipeViewModelFactory ?? throw new ArgumentNullException(nameof(recipeViewModelFactory));
        _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
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

        if (recipe.FoodCategories != null)
        {
            foreach (var category in recipe.FoodCategories)
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
	    .ThenInclude(x => x.CategoryNavigation)
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
        var recipe = await _dbInfo.Recipes.Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == recipeId);
        if (recipe is null)
            return new ManagerActionResult(ResultEnum.BadRequest, _recipeNotFound);

        recipe.Users.Where(x => x.Login == user).ToList().ForEach(x => recipe.Users.Remove(x));
        await _dbInfo.SaveChangesAsync();

        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<RecipesComment[]>> GetRecipeCommentsPagedAsync(int recipeId, int page, int pageSize)
    {
        IQueryable<RecipesComment> comments = _dbInfo.RecipesComments.Where(x => x.Recipe == recipeId).OrderByDescending(x => x.Date);
        return new ManagerActionResult<RecipesComment[]>(await _pageingManager.GetPagedInfo(comments, page, pageSize).ToArrayAsync(), ResultEnum.OK);
    }

    public async Task<IManagerActionResult<RecipeOutViewModel[]>> GetRecipesPagedAsync(
        int page, int pageSize, string searchName, int[] ingredientsIds, int[] foodCategoriesIds, int[] fridgesIds, string user)
    {
        IQueryable<Recipe> data = _dbInfo.Recipes.Include(x => x.Categories).Include(x => x.RecipiesIngredients);
        if(!searchName.IsNullOrEmpty())
            data = data.Where(x => x.Name.Contains(searchName));
        if(ingredientsIds is not null && ingredientsIds.Length > 0) 
        {
            data = data.Where(x => ingredientsIds.All(ingredientId => x.RecipiesIngredients.Any(x => x.Category == ingredientId)));
        }
        if(foodCategoriesIds is not null && foodCategoriesIds.Length > 0) 
        {
            data = data.Where(x => foodCategoriesIds.All(categoryId => x.Categories.Any(x => x.Id == categoryId)));
        }

        if(fridgesIds is not null && fridgesIds.Length > 0) 
        {
            var fridgesIngredients = _dbInfo.Fridges
                .Where(f => fridgesIds.Contains(f.Id) && f.UserLogin == user)
                .Include(f => f.FridgeProducts)
                .SelectMany(f => f.FridgeProducts)
                .GroupBy(fp => fp.Product.Category)
                .Select(g => new
                {
                    CategoryId = (int)g.Key,
                    Quantity = g.Sum(fp => (double)fp.Ammount)
                });


            data = data.Where(recipe =>
                recipe.RecipiesIngredients.All(ingredient =>
                    fridgesIngredients.Any(ri =>
                        ri.CategoryId == ingredient.Category && ri.Quantity >= ingredient.Quantity
                    )
                )
            );
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

    public async Task<IManagerActionResult> ReplaceIngredientsAsync(int recipeId, string user, (int categoryId, double quantity)[] ingredients)
    {
        using var transaction = await _dbInfo.Database.BeginTransactionAsync();
        var recipe = await _dbInfo.Recipes.FirstOrDefaultAsync(x => x.Id == recipeId && x.Author == user);
        if(recipe is null)
            return new ManagerActionResult(ResultEnum.BadRequest);
        var oldIngredients = await _dbInfo.RecipiesIngredients.Where(x => x.Reciepie == recipeId).ToArrayAsync();
        foreach(var ingredient in oldIngredients)
            _dbInfo.RecipiesIngredients.Remove(ingredient);
        foreach(var ingredient in ingredients)
            await _dbInfo.RecipiesIngredients.AddAsync(new RecipiesIngredient
            {
                Reciepie = recipeId,
                Category = ingredient.categoryId,
                Quantity = ingredient.quantity
            });
        await _dbInfo.SaveChangesAsync();
        await transaction.CommitAsync();
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
	    .ThenInclude(x => x.CategoryNavigation)
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

    public async Task<IManagerActionResult<int>> ForkRecipeAsync(int recipeId, string user)
    {
        var oryginalRecipe = await _dbInfo.Recipes
            .Include(x => x.Categories)
            .Include(x => x.RecipiesIngredients)
            .Include(x => x.RecipiesImages).FirstOrDefaultAsync(x => x.Id == recipeId);
        if (oryginalRecipe is null)
            return new ManagerActionResult<int>(-1, ResultEnum.BadRequest, _recipeNotFound);
        var newRecipe = new Recipe()
        {
            Name = oryginalRecipe.Name,
            Description = oryginalRecipe.Description,
            Author = user
        };
        using var transaction = await _dbInfo.Database.BeginTransactionAsync();
        await _dbInfo.Recipes.AddAsync(newRecipe);
        await _dbInfo.SaveChangesAsync();

        newRecipe.MarkdownPath = _pathProvider.GetMarkdownPath(newRecipe.Id);
        using var reader = new StreamReader(_fileProvider.GetFileByPath(oryginalRecipe.MarkdownPath), Encoding.UTF8);
        await SaveMarkdownAsync(newRecipe.Id,         reader.ReadToEnd());

        var mainImage = await GetMainImageAsync(oryginalRecipe.Id);

        if (mainImage is not null) 
        {
        
            var path = _pathProvider.GetMainImagePath(newRecipe.Id, mainImage.Name);
            var imageBytes = _fileProvider.GetFileByPath(mainImage.ImagePath);
            await _fileSaver.SaveFileAsync(path , imageBytes);
            await _dbInfo.RecipiesImages.AddAsync(new RecipiesImage()
            {
                Recipe = newRecipe.Id,
                Name = mainImage.Name,
                ImagePath = path
            });
        }
        
        foreach(var image in oryginalRecipe.RecipiesImages)
        {
            if(image.ImagePath.Contains(_pathProvider.GetMainImagePathPrefix(oryginalRecipe.Id)))
                continue;
            
            var path = _pathProvider.GetImagePath(newRecipe.Id, image.Name);
            var imageBytes = _fileProvider.GetFileByPath(image.ImagePath);
            await _fileSaver.SaveFileAsync(path , imageBytes);
            await _dbInfo.RecipiesImages.AddAsync(new RecipiesImage()
            {
                Recipe = newRecipe.Id,
                Name = image.Name,
                ImagePath = path
            });
        }
        foreach(var ingredient in oryginalRecipe.RecipiesIngredients)
        {

            await _dbInfo.RecipiesIngredients.AddAsync(new RecipiesIngredient()
            {
                Reciepie = newRecipe.Id,
                Quantity = ingredient.Quantity,
                Category = ingredient.Category
            });
        }
        foreach(var category in oryginalRecipe.Categories) 
            newRecipe.Categories.Add(category);

        await _dbInfo.SaveChangesAsync();
        await transaction.CommitAsync();
        return new ManagerActionResult<int>(newRecipe.Id, ResultEnum.OK);
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
