public interface IDateTimeProvider
{
    DateTime GetCurrentDateTime();
    long GetCurrentMiliseconds();
}