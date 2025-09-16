using Microsoft.Extensions.Logging;

namespace Mohr.Jonas.Spaceshot.Spaceshot.IOC;

public class PayloadEvictor
{
    private readonly ILogger _logger;
    private readonly DiContainer _container;

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