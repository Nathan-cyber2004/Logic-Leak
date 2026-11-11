using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject levelSelectMenu;
    [SerializeField] private AudioClip backgroundMusic;

    private static AudioSource persistentMusic;

    void Start()
    {
        if (levelSelectMenu != null)
        {
            levelSelectMenu.SetActive(false);
        }

        if (persistentMusic == null && backgroundMusic != null)
        {
            GameObject musicObject = new GameObject("Background Music");
            DontDestroyOnLoad(musicObject);

            persistentMusic = musicObject.AddComponent<AudioSource>();
            persistentMusic.clip = backgroundMusic;
            persistentMusic.loop = true;
            persistentMusic.playOnAwake = false;
            persistentMusic.spatialBlend = 0f;
            persistentMusic.Play();
        }
    }

    public void LevelSelect()
    {
        levelSelectMenu.gameObject.SetActive(true);
    }
    public void exitLevelSelect()
    {
        levelSelectMenu.gameObject.SetActive(false);
    }
    public void startLevelOne()
    {
        SceneManager.LoadScene("LevelOne");
    }
    public void startLevelTwo()
    {
        SceneManager.LoadScene("LevelTwo");
    }
    public void startLevelThree()
    {
        SceneManager.LoadScene("LevelThree");
    }
    public void startLevelFour()
    {
        SceneManager.LoadScene("LevelFour");
    }
    public void startLevelFive()
    {
        SceneManager.LoadScene("LevelFive");
    }

}
