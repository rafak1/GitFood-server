
namespace Server.Logic.Abstract;

public interface IDatabaseErrorHanlder
{
    string HandleSqlExceptions(Exception ex);
}