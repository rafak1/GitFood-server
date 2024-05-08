using Microsoft.AspNetCore.Http;

namespace Server.ViewModels.Recipes;

public class RecipeImageViewModel
{
    public required string Name { get; set; }
    public required IFormFile Image {get; set;}
}