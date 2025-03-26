using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PitScript : MonoBehaviour
{
    public GameObject gameOverText; // Assign in the Inspector
    public GameAnalytics gameAnalytics;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player fell into the pit! Game Over.");
            gameAnalytics.EndLevelDeath(collision.transform.position.x, collision.transform.position.y);
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
                player.enabled = false;
            
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

        MenuController.showLevelsPanel = true;
        SceneManager.LoadScene("MainMenuScene");
    }
}
