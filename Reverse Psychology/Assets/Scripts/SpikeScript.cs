using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SpikeScript : MonoBehaviour
{
    public GameObject gameOverText; // Assign in the Inspector
    public GameAnalytics gameAnalytics;
    public GameObject button;
    public GameObject PauseMenu;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player touched spike! Game Over.");
            gameAnalytics.EndLevelDeath(collision.transform.position.x, collision.transform.position.y);
            //PlayerController player = collision.GetComponent<PlayerController>();
            //if (player != null)
            //    player.enabled = false;
            //Time.timeScale = 0f;

            //if (button.GetComponent<ButtonScript>().checkpointCreated) // CHANGED
            //{
                PauseMenu.GetComponent<PauseManager>().RestartFromCheckpoint();
            //}
            //else
            //{
            //    // Start Coroutine to wait and exit the game
            //    StartCoroutine(GameOver());
            //}

            
        }
    }

    IEnumerator GameOver()
    {
        if (gameOverText != null)
            gameOverText.SetActive(true);
        // Wait for 2 seconds in real time, even though the game is paused.
        yield return new WaitForSecondsRealtime(2f);
        if (gameOverText != null)
            gameOverText.SetActive(false);

        MenuController.showLevelsPanel = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
