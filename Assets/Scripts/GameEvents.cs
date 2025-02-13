using System;

public static class GameEvents
{
    public static event Action OnBoardUpdated;

    public static void BoardUpdated()
    {
        OnBoardUpdated?.Invoke();
    }
}