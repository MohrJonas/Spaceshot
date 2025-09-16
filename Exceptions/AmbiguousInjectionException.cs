namespace Mohr.Jonas.Spaceshot.Exceptions;

public class AmbiguousInjectionException(object key)
    : Exception($"Ambiguous injection key {key}");