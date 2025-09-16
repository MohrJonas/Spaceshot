#region

using Microsoft.Extensions.Logging;

#endregion

namespace Mohr.Jonas.Spaceshot.IOC;

public class PayloadEvictor
{
    private readonly DiContainer _container;
    private readonly ILogger _logger;

    internal PayloadEvictor(ILogger logger, DiContainer container)
    {
        _logger = logger;
        _container = container;
    }

    public bool Evict<TEvict>()
    {
        _logger.LogInformation("Evicting value of type {}", typeof(TEvict));
        return _container.EvictValueForType(typeof(TEvict));
    }

    public bool EvictKeyed(string key)
    {
        _logger.LogInformation("Evicting value of key {}", key);
        return _container.EvictValueForKey(key);
    }
}