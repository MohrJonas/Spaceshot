namespace Mohr.Jonas.Spaceshot;

public class Spaceship
{
    private readonly Spaceshot _spaceshot;

    internal Spaceship(Spaceshot spaceshot)
    {
        _spaceshot = spaceshot;
    }

    public void AbortMission(bool waitForCompletion = false)
    {
        _spaceshot.AbortMission(waitForCompletion);
    }

    public async Task AbortMissionAsync(bool waitForCompletion = false)
    {
        await _spaceshot.AbortMissionAsync(waitForCompletion);
    }
}