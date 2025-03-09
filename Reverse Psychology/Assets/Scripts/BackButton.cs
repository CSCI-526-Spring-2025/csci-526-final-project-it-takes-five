using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{

    public void OnBackToTutorialClicked()
    {
        Debug.Log("Back button clicked - attempting to load MainTutorialMenu");
        MenuController.showTutorialsPanel = true;
        MenuController.showLevelsPanel = false;
        SceneManager.LoadScene("MainMenuScene");
    }
    public void OnBackToLevelsClicked()
    {
        MenuController.showLevelsPanel = true;
        MenuController.showTutorialsPanel = false;
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

    public void OnRestart1Clicked()
    {
        SceneManager.LoadScene("Tutorial1Scene");
    }
    public void OnRestart2Clicked()
    {
        SceneManager.LoadScene("Tutorial2Scene");
    }
    public void OnRestart3Clicked()
    {
        SceneManager.LoadScene("Tutorial3Scene");
    }

    public void OnBackToMainMenuClicked()
    {
        Debug.Log("Back button clicked - attempting to loadMainMenu");
        MenuController.showTutorialsPanel = false;
        SceneManager.LoadScene("MainMenuScene");
    }

}
