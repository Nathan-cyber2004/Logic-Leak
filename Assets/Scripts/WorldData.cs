using UnityEngine;

[CreateAssetMenu(menuName = "LogicLeak/World")]
public class WorldData : ScriptableObject
{
    public string worldName, concept;
    public int levelIndex;
    public string nextScene = "MainMenu";
    public Sprite monster;
    public bool adAfter;
    public int playerHP = 100;
    public int battleHP = 100, bossHP = 160;
    public int correctDamage = 25, comboBonusDamage = 5;
    public int wrongDamage = 15, bossWrongDamage = 25;
    public float questionTime = 15f, bossQuestionTime = 10f;
    public LessonPage[] lessons;            // 3 short cards
    public BuildChallenge[] buildChallenges; // your old slot puzzles
    public Question[] questions;
    public Question[] bossQuestions;
}

[System.Serializable]
public class LessonPage { [TextArea] public string text; [TextArea] public string code; }
