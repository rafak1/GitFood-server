using Server.Logic.Abstract;

namespace Server.Logic;

public class FileSaver : IFileSaver
{
    public void DeleteFile(string path)=> File.Delete(path);

    public async Task SaveFileAsync(string path, Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var writer = new FileStream(path, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(writer);
        writer.Close();
    }
}