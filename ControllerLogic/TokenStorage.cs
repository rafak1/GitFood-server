public class TokenStorage : ITokenStorage
{
    private readonly Dictionary<string, (long CreationTime , string User)> _tokens = new Dictionary<string, (long , string)>();

    private readonly ITokenConfigProvider _tokenConfigProvider;

    private readonly IDateTimeProvider _dateTimeProvider;

    private long lastPurge = 0;

    public TokenStorage(ITokenConfigProvider tokenConfigProvider, IDateTimeProvider dateTimeProvider)
    {
        _tokenConfigProvider = tokenConfigProvider ?? throw new ArgumentNullException(nameof(tokenConfigProvider));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        lastPurge = _dateTimeProvider.GetCurrentMiliseconds();
    }

    public void AddToken(string token, string user)
    {
        var currentTime = _dateTimeProvider.GetCurrentMiliseconds();
        PurgeIfTime(currentTime);
        _tokens.Add(token, (currentTime, user));
    }

    #nullable enable
    public string? GetUser(string token)
    {
        PurgeIfTime(_dateTimeProvider.GetCurrentMiliseconds());
        if (_tokens.ContainsKey(token))
        {
            return _tokens[token].User;
        }
        return null;
    }

    private void PurgeIfTime(long currentTime){
        if (currentTime - lastPurge <= _tokenConfigProvider.GetJwtPurgeInterval()) return;
        lastPurge = currentTime;
        _tokens.Where(t => currentTime - t.Value.CreationTime > _tokenConfigProvider.GetJwtExpireMinutes() * 60 * 1000)
            .ToList()
            .ForEach(t => _tokens.Remove(t.Key));
    }
}