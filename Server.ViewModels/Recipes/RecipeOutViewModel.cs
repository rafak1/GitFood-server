namespace Server.ViewModels.Recipes;


public class RecipeOutViewModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Author { get; set; }
    public required string MarkdownPath { get; set; }
    public string? TitleImage { get; set; } 
    public List<RecipeIngredientViewModel>? Ingredients { get; set; }
    public List<int>? Categories { get; set; }
    public List<string>? Likes { get; set; }
    public List<RecipeCommentViewModel>? Comments { get; set; }
    public List<string>? ImagePaths { get; set; }
}