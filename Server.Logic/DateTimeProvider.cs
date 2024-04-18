using Server.Logic.Abstract;

namespace Server.Logic;

public class DateTimeProvider : IDateTimeProvider
{
    private readonly DateTime _baseTime = new(1970, 1, 1);

    public DateTime GetCurrentDateTime() => DateTime.Now;

    public long GetCurrentMiliseconds()
    {
        return (long)(DateTime.Now - _baseTime).TotalMilliseconds;
    }
}