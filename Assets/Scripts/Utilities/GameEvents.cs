using System;

public static class GameEvents
{
    public static event Action OnBoardUpdated;
    public static event Action OnGameStateUpdated;

    public static void BoardUpdated()
    {
        OnBoardUpdated?.Invoke();
    }

    public static void GameStateUpdated()
    {
        OnGameStateUpdated?.Invoke();
    }
}