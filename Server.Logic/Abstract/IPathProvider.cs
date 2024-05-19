namespace Server.Logic.Abstract;

public interface IPathProvider
{
    string GetImagePath(int recipeId, string imageName);
    string GetMarkdownPath(int recipeId);
}