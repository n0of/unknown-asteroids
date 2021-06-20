public static class GameState
{
    public static int Health { get; set; }
    public static int Score { get; set; }
    public static int Level { get; set; }
    public static bool IsKeyboardOnly { get; set; }
    public static bool IsPaused { get; set; }
    public static GameScriptableObject Settings { get; set; }
    public static void InitState(GameScriptableObject settings)
    {
        Health = settings.PlayerHealth;
        Score = 0;
        Level = 0;
        IsKeyboardOnly = true;
        IsPaused = true;
        Settings = settings;
    }
    public static void ResetState(GameScriptableObject settings)
    {
        Health = settings.PlayerHealth;
        Level = 0;
        Score = 0;
    }
}
