using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Hint : MonoBehaviour
{
    public GameObject PickupText;
    public GameObject JumpText;
    public GameObject IntroText;
    public GameObject DropText;
    public GameObject Overlay;

    public GameObject OrbStackCanvas;
    public GameObject ButtonArrow;
    public GameObject DoorArrow;

    private bool isNearOrb = false;
    private bool isNearObstacle = false;
    private bool gameStarted = false;
    private bool orbHintShown = false;
    private bool dropHintShown = false;
    private bool pressedButton = false;

    void Start()
    {
        // Initially hide hint texts
        if (PickupText != null) PickupText.SetActive(false);
        if (JumpText != null) JumpText.SetActive(false);
        if (DropText != null) DropText.SetActive(false);

        // Show intro text and arrows
        if (IntroText != null) IntroText.SetActive(true);
        if (ButtonArrow != null) ButtonArrow.SetActive(true);
        if (DoorArrow != null) DoorArrow.SetActive(true);

        // Freeze the game
        Time.timeScale = 0;
        if (Overlay != null) Overlay.SetActive(true);
    }

    void Update()
    {
        // Wait for key press to start game from intro
        if (!gameStarted && Time.timeScale == 0 && Input.anyKeyDown)
        {
            Time.timeScale = 1;
            if (Overlay != null) Overlay.SetActive(false);
            gameStarted = true;
            if (IntroText != null) IntroText.SetActive(false);
            // Hide arrows
            if (ButtonArrow != null) ButtonArrow.SetActive(false);
            if (DoorArrow != null) DoorArrow.SetActive(false);
        }

        // Pickup orb with mouse click when near orb
        if (isNearOrb && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.O)))
        {
            if (PickupText != null) PickupText.SetActive(false);
            isNearOrb = false;
            Time.timeScale = 1;
            if (Overlay != null) Overlay.SetActive(false);

            // wait 1 second
            if (!dropHintShown) StartCoroutine(ShowDropHint());

            
        }

        // Drop orb with right mouse click
        if (dropHintShown && DropText.activeSelf && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.P)))
        {
            if (DropText != null) DropText.SetActive(false);
            Time.timeScale = 1;
            if (Overlay != null) Overlay.SetActive(false);
        }

        // Resume game after jump (space key) near button
        if (isNearObstacle && Input.anyKeyDown)
        {
            if (JumpText != null) JumpText.SetActive(false);
            isNearObstacle = false;
            Time.timeScale = 1;
            if (Overlay != null) Overlay.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BlueOrb") && !orbHintShown)
        {
            orbHintShown = true;
            if (PickupText != null) PickupText.SetActive(true);
            OrbStackCanvas.transform.SetAsLastSibling();
            isNearOrb = true;
            Time.timeScale = 0; // Freeze the game
            if (Overlay != null) Overlay.SetActive(true);
        }

        if (collision.CompareTag("JumpTrigger") && pressedButton)
        {
            if (JumpText != null) JumpText.SetActive(true);
            isNearObstacle = true;
            Time.timeScale = 0; // Freeze the game
            if (Overlay != null) Overlay.SetActive(true);
        }

        if (collision.CompareTag("Button")) {
            pressedButton = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BlueOrb"))
        {
            if (PickupText != null) PickupText.SetActive(false);
            isNearOrb = false;
        }
    }

    IEnumerator ShowDropHint()
    {
        yield return new WaitForSeconds(1f);
        if (DropText != null) {
            dropHintShown = true;
            DropText.SetActive(true);
            Time.timeScale = 0;
            if (Overlay != null) Overlay.SetActive(true);
        }
    }
}
