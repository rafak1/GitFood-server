using Server.Logic.Abstract;

namespace Server.Logic;

internal class FileProvider : IFileProvider
{
    public Stream GetFileByPath(string path)
    {
        return File.OpenRead(path);
    }
}