using Server.Logic.Abstract;

namespace Server.Logic;

internal class PathProvider : IPathProvider
{
    private const string _fileFolder = "./recipe_files/";

    public string GetImagePath(int recipeId, string imageName)
     => $"{_fileFolder}recipe_image_{recipeId}_{imageName}";

    public string GetMarkdownPath(int recipeId)
     => $"{_fileFolder}recipe_{recipeId}.md";

    public string GetMainImagePath(int recipeId, string fileName)
     => $"{GetMainImagePathPrefix(recipeId)}_{fileName}";

    public string GetMainImagePathPrefix(int recipeId)
     => $"{_fileFolder}_recipe_main_{recipeId}";
}