namespace Server.ViewModels.Recipes;


public class RecipeOutViewModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Author { get; set; }
    public required string MarkdownPath { get; set; }
    public string? TitleImage { get; set; } 
    public int NumberOfLikes;
    public bool IsLiked;
    public List<int>? Categories { get; set; }
}