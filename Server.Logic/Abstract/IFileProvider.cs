namespace Server.Logic.Abstract;

public interface IFileProvider
{
    Stream GetFileByPath(string path);
}