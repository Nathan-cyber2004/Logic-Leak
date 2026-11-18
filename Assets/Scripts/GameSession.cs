using UnityEngine;

public static class GameSession
{
    public static SaveData Save { get; private set; } = SaveData.Load();
    public static int WorldIndex { get; private set; }
    public static bool IsBoss { get; set; }
    public static WorldData CurrentWorld { get; private set; }
    private static int correct, misses;

    public static void StartWorld(int worldIndex, WorldData world = null)
    {
        WorldIndex = Mathf.Clamp(worldIndex, 0, 4);
        CurrentWorld = world;
        IsBoss = false;
        correct = misses = 0;
    }

    public static void AddResults(int correctAnswers, int wrongAnswers)
    {
        correct += Mathf.Max(0, correctAnswers);
        misses += Mathf.Max(0, wrongAnswers);
    }

    public static void CompleteWorld()
    {
        Save.progress = Mathf.Max(Save.progress, WorldIndex + 1);
        Save.xp += Mathf.Max(0, correct * 10 - misses * 2);
        int stars = correct >= 9 ? 3 : correct >= 6 ? 2 : 1;
        if (Save.stars == null || Save.stars.Length < 5) Save.stars = new int[5];
        Save.stars[WorldIndex] = Mathf.Max(Save.stars[WorldIndex], stars);
        Save.Save();
    }
}
