using Microsoft.AspNetCore.Mvc;
using Server.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Token;
using Server.ViewModels.Products;

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
    public async Task<IActionResult> CreateRecipe(RecipeViewModel recipe)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _recipeManager.CreateRecipeAsync(recipe, user)).MapToActionResult();
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _recipeManager.DeleteRecipeAsync(id, user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/getById")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _recipeManager.GetRecipeByIdAsync(id, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/addComment")]
    public async Task<IActionResult> AddComment(int recipeId, string comment)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _recipeManager.AddCommentAsync(recipeId, comment, user)).MapToActionResult();
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/removeComment")]
    public async Task<IActionResult> RemoveComment(int commentId)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _recipeManager.RemoveCommentAsync(commentId, user)).MapToActionResult();
    }

    [HttpPost]
    [Route($"{_controllerRoute}/like")]
    public async Task<IActionResult> LikeRecipe(int recipeId)
    {
        var user = GetUser(Request.Headers.Authorization);
        if (user == null)
            return BadRequest(_userNotFound);

        return (await _recipeManager.LikeRecipeAsync(recipeId, user)).MapToActionResult();
    }

}