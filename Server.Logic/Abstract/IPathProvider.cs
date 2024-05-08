namespace Server.Logic;

public interface IPathProvider
{
    string GetImagePath(int recipeId, string imageName, string user, string recipeName);
    string GetMarkdownPath(string user, string recipeName, int recipeId);
}