namespace Mohr.Jonas.Spaceshot.Abstractions;

/// <summary>
///     Base interface for classes providing dependency injection
/// </summary>
public interface IDiContainer
{
    /// <summary>
    ///     Get the given injectable object for the given type
    /// </summary>
    /// <param name="injectedClassType">The class that is currently created via dependency injection</param>
    /// <param name="type">The type to get the object for</param>
    /// <returns>The injectable object</returns>
    public object GetValueForType(Type injectedClassType, Type type);

    /// <summary>
    ///     Get the given injectable object for the given key
    /// </summary>
    /// <param name="injectedClassType">The class that is currently created via dependency injection</param>
    /// <param name="key">The key to get the object for</param>
    /// <returns>The injectable object</returns>
    public object GetValueForKey(Type injectedClassType, string key);

    public bool EvictValueForType(Type type);

    public bool EvictValueForKey(string key);
}