using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SpikeScript : MonoBehaviour
{
    public GameObject gameOverText; // Assign in the Inspector
    public GameAnalytics gameAnalytics;
    private int maxTutorialLevel = 3;
    private int maxLevels = 1;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit spikes! Game Over.");  
            gameAnalytics.EndLevelDeath(collision.transform.position.x, collision.transform.position.y);

            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.abilityEnd();
                player.enabled = false;
            }
            // Start Coroutine to wait and exit the game
            StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver()
    {
        if (gameOverText != null)
            gameOverText.SetActive(true);
        yield return new WaitForSeconds(5f);
        if (gameOverText != null)
            gameOverText.SetActive(false);
        Debug.Log("Game Over ");
        MenuController.showLevelsPanel = true;

        SceneManager.LoadScene("MainMenuScene");
        
    }
}
