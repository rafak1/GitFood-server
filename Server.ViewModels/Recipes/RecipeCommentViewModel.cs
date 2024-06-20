namespace Server.ViewModels.Recipes;


public class RecipeCommentViewModel
{
    public required string Author { get; set; }

    public int Id { get; set; }

    public required string Message { get; set; }

    public int? Likes { get; set; }

    public DateTime? Date { get; set; }
}