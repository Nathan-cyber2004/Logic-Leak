using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject levelSelectMenu;

    void Start()
    {
        levelSelectMenu.gameObject.SetActive(false);
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
