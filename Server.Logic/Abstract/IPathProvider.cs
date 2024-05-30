namespace Server.Logic.Abstract;

public interface IPathProvider
{
    string GetImagePath(int recipeId, string imageName);
    string GetMarkdownPath(int recipeId);
    string GetMainImagePath(int recipeId, string fileName);
    string GetMainImagePathPrefix(int recipeId);
}