using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Token;
using Server.ViewModels.Recipes;
using Microsoft.AspNetCore.Http;
using Server.ViewModels;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class RecipeController : BaseController
{
    private const string _controllerRoute = "/recipe";
    private readonly IRecipeManager _recipeManager;

    public RecipeController(IRecipeManager recipeManager, ITokenStorage tokenStorage) : base(tokenStorage)
    {
        _recipeManager = recipeManager ?? throw new ArgumentNullException(nameof(recipeManager));
    }

    [HttpPost]
    [Route($"{_controllerRoute}/create")]
    public async Task<IActionResult> CreateRecipe([FromBody] RecipeViewModel recipe)
    {
        var user = GetUser();
        return (await _recipeManager.CreateRecipeAsync(recipe, user)).MapToActionResult();
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [Route($"{_controllerRoute}/addOrUpdateMainPhoto")]
    public async Task<IActionResult> AddOrUpdateMainPhoto([FromQuery] int recipeId, [FromForm] IFormFile[] image)
    {
        var user = GetUser();
        using var stream = new MemoryStream();
        var firstImage = image[0];
        await firstImage.CopyToAsync(stream);
        return (await _recipeManager.AddOrUpdateMainPhoto(recipeId, user, stream, firstImage.FileName)).MapToActionResult();
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [Route($"{_controllerRoute}/addPhotos")]
    public async Task<IActionResult> AddPhotos([FromQuery] int recipeId, [FromForm] IFormFile[] images)
    {
        var user = GetUser();
        var imageList = new List<RecipeImageViewModel>();
        try{
            foreach(var image in images) 
            {
                var stream = new MemoryStream();
                await image.CopyToAsync(stream);
                imageList.Add(new RecipeImageViewModel
                {
                    Image = stream,
                    Name = image.FileName,
                });
            }
            return (await _recipeManager.AddImagesAsync(recipeId, imageList.ToArray(), user)).MapToActionResult();
        }
        finally
        {
            foreach(var image in imageList)
                image.Image.Dispose();
        }
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var user = GetUser();
        return (await _recipeManager.DeleteRecipeAsync(id, user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/getById")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
        var user = GetUser();
        return (await _recipeManager.GetRecipeByIdAsync(id, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/addComment")]
    public async Task<IActionResult> AddComment(int recipeId, string comment)
    {
        var user = GetUser();
        return (await _recipeManager.AddCommentAsync(recipeId, comment, user)).MapToActionResult();
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/removeComment")]
    public async Task<IActionResult> RemoveComment(int commentId)
    {
        var user = GetUser();
        return (await _recipeManager.RemoveCommentAsync(commentId, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/like")]
    public async Task<IActionResult> LikeRecipe(int recipeId)
    {
        var user = GetUser();
        return (await _recipeManager.LikeRecipeAsync(recipeId, user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/getCommentsPaged")]
    public async Task<IActionResult> GetCommentsPaged(int recipeId, int page, int pageSize)
    {
        var user = GetUser();
        return (await _recipeManager.GetRecipeCommentsPagedAsync(recipeId, page, pageSize)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/getPaged")]
    public async Task<IActionResult> GetRecepiesPaged(int page, int pageSize, [FromBody]RecipeSearchViewModel searchParams)
    {
        var user = GetUser();
        return (await _recipeManager.GetRecipesPagedAsync(
            page, pageSize, searchParams.SearchName, searchParams.IngredientsIds, searchParams.FoodCategoriesIds, searchParams.FridgeIds, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/deleteImages")]
    public async Task<IActionResult> DeleteImages(int recipeId, [FromBody] string[] imageNames)
    {
        var user = GetUser();
        return (await _recipeManager.DeleteImagesAsync(recipeId, imageNames, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/updateMarkdown")]
    public async Task<IActionResult> UpdateMarkdown(int recipeId, [FromBody] MarkdownViewModel markdown)
    {
        var user = GetUser();
        return (await _recipeManager.UpdateMarkdownAsync(recipeId, markdown.Markdown, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/updateDescription")]
    public async Task<IActionResult> UpdateDescription(int recipeId, RecipeDescriptionViewModel model)
    {
        var user = GetUser();
        return (await _recipeManager.UpdateDescriptionAsync(recipeId, model.Description, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/updateIngredient")]
    public async Task<IActionResult> UpdateIngredient(int recipeId, int categoryId, double quantity)
    {
        var user = GetUser();
        return (await _recipeManager.UpdateIngredientsAsync(recipeId, categoryId, quantity, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/replaceIngredients")]
    public async Task<IActionResult> ReplaceIngredients(int recipeId, [FromBody] (int categoryId, double quantity)[] ingredients)
    {
        var user = GetUser();
        return (await _recipeManager.ReplaceIngredientsAsync(recipeId, user, ingredients)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/unlike")]
    public async Task<IActionResult> UnlikeRecipe(int recipeId)
    {
        var user = GetUser();
        return (await _recipeManager.UnlikeRecipeAsync(recipeId, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/updateName")]
    public async Task<IActionResult> UpdateName(int recipeId, string name)
    {
        var user = GetUser();
        return (await _recipeManager.UpdateRecipeNameAsync(recipeId, name, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/addReference")]
    public async Task<IActionResult> AddReference(int recipeId, int referenceId, double multiplayer)
    {
        var user = GetUser();
        return (await _recipeManager.AddReferenceToRecipeAsync(recipeId, referenceId, multiplayer, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/removeReference")]
    public async Task<IActionResult> RemoveReference(int recipeId, int referenceId, double multiplayer)
    {
        var user = GetUser();
        return (await _recipeManager.RemoveReferenceToRecipeAsync(recipeId, referenceId, user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/getRecipeDetails")]
    public async Task<IActionResult> GetRecipeDetails([FromQuery] int recipeId)
    {
        var user = GetUser();
        return (await _recipeManager.GetRecipeDetailsAsync(recipeId, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/updateFoodCategories")]
    public async Task<IActionResult> UpdateCategories([FromQuery] int recipeId, [FromBody] int[] categoryIds)
    {
        var user = GetUser();
        return (await _recipeManager.UpdateRecipeCategoriesAsync(recipeId, user, categoryIds)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/forkRecipe")]
    public async Task<IActionResult> ForkRecipe([FromQuery] int recipeId)
    {
        var user = GetUser();
        return (await _recipeManager.ForkRecipeAsync(recipeId, user)).MapToActionResult();
    }

}
