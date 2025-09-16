namespace Mohr.Jonas.Spaceshot.Exceptions;

public class NoInjectableFoundException(object key)
    : Exception($"No element with key {key} exists");