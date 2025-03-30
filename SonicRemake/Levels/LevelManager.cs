namespace SonicRemake.Levels;

public static class LevelManager
{
    public static Level Active { get; private set; }
    private static bool _levelChanged = false;

    public static void LoadLevel(Level level)
    {
        Active = level;
        _levelChanged = true;
    }

    public static bool HasLevelChanged()
    {
        if (_levelChanged)
        {
            _levelChanged = false;
            return true;
        }

        return false;
    }
}
