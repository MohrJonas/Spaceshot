namespace Mohr.Jonas.Spaceshot.Spaceshot.IOC;

public sealed class DiInjector
{
    private readonly DiContainer _container;
    private readonly Type _injectedClassType;

    internal DiInjector(Type injectedClassType, DiContainer container)
    {
        _injectedClassType = injectedClassType;
        _container = container;
    }

    public TInjectable Inject<TInjectable>()
    {
        return (TInjectable)_container.GetValueForType(_injectedClassType, typeof(TInjectable));
    }

    public TInjectable InjectKeyed<TInjectable>(string key)
    {
        return (TInjectable)_container.GetValueForKey(_injectedClassType, key);
    }
}