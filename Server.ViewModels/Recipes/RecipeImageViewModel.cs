using Microsoft.AspNetCore.Http;

namespace Server.ViewModels.Recipes;

public class RecipeImageViewModel
{
    public string Name { get; set; }
    public Stream Image {get; set;}
}