using Server.Logic.Abstract;

namespace Server.Logic;

internal class PathProvider : IPathProvider
{
    public string GetImagePath(int recipeId, string imageName, string user, string recipeName)
     => $"./recipes/{user}/{recipeName}_{recipeId}_{imageName}.png";

    public string GetMarkdownPath(string user, string recipeName, int recipeId)
     => $"./recipes/{user}/{recipeName}_{recipeId}.md";
}