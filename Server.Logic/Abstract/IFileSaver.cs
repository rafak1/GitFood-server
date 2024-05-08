namespace Server.Logic.Abstract;

public interface IFileSaver
{
    Task SaveFileAsync(string path, Stream fileStream);

    void DeleteFile(string path);
}