using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int progress, xp;
    public int[] stars = new int[5];
    public bool timer = true;

    public static SaveData Load()
    {
        string j = PlayerPrefs.GetString("save", "");
        if (string.IsNullOrEmpty(j)) return new SaveData();
        try
        {
            SaveData data = JsonUtility.FromJson<SaveData>(j);
            if (data == null) return new SaveData();
            if (data.stars == null || data.stars.Length < 5) data.stars = new int[5];
            data.progress = Mathf.Clamp(data.progress, 0, 5);
            data.xp = Mathf.Max(0, data.xp);
            return data;
        }
        catch (System.ArgumentException)
        {
            return new SaveData();
        }
    }
    public void Save() { PlayerPrefs.SetString("save", JsonUtility.ToJson(this)); PlayerPrefs.Save(); }
}
