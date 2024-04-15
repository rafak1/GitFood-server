public class TokenStorage : ITokenStorage
{
    private readonly Dictionary<string, (long , string)> _tokens = new Dictionary<string, (long , string)>();

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
        long currentTime = _dateTimeProvider.GetCurrentMiliseconds();
        PurgeIfTime(currentTime);
        _tokens.Add(token, (currentTime, user));
    }

    public string getUser(string token)
    {
        PurgeIfTime(_dateTimeProvider.GetCurrentMiliseconds());
        if (_tokens.ContainsKey(token))
        {
            return _tokens[token].Item2;
        }
        return null;
    }

    private void PurgeIfTime(long currentTime){
        if (currentTime - lastPurge > _tokenConfigProvider.GetJwtPurgeInterval())
        {
            lastPurge = currentTime;
            var toRemove = new List<string>();
            foreach (var entry in _tokens)
            {
                if (currentTime - entry.Value.Item1 > _tokenConfigProvider.GetJwtExpireMinutes() * 60)
                {
                    toRemove.Add(entry.Key);
                }
            }
            foreach (var key in toRemove)
            {
                _tokens.Remove(key);
            }
        }
    }
}