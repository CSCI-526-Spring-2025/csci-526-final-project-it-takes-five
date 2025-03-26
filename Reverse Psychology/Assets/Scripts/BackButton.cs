using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public GameObject levelsPanel;

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnRestartLevel1Clicked()
    {
        SceneManager.LoadScene("Level1");
    }
    public void OnRestartLevel2Clicked()
    {
        SceneManager.LoadScene("Level2");
    }
    public void OnRestartLevel3Clicked()
    {
        SceneManager.LoadScene("Level3");
    }
    public void OnRestartLevel4Clicked()
    {
        SceneManager.LoadScene("Level4");
    }
    public void OnRestartLevel5Clicked()
    {
        SceneManager.LoadScene("Level5");
    }
    public void OnRestartLevel6Clicked()
    {
        SceneManager.LoadScene("Level6");
    }
}
