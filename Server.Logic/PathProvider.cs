using Server.Logic.Abstract;

namespace Server.Logic;

internal class PathProvider : IPathProvider
{
    public string GetImagePath(int recipeId, string imageName)
     => $"./recipe_files/recipe_image_{recipeId}_{imageName}";

    public string GetMarkdownPath(int recipeId)
     => $"./recipe_files/recipe_{recipeId}.md";
}