namespace Mohr.Jonas.Spaceshot.Spaceshot.Abstractions;

public interface IAsyncBackgroundService
{
    public Task SetupAsync();
    public Task TeardownAsync();
}