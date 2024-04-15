public class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetCurrentDateTime()
    {
        return DateTime.Now;
    }

    public long GetCurrentMiliseconds()
    {
        return (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
}