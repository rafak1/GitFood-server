namespace Server.Logic.Abstract;

public interface IDateTimeProvider
{
    DateTime GetCurrentDateTime();
    long GetCurrentMiliseconds();
}