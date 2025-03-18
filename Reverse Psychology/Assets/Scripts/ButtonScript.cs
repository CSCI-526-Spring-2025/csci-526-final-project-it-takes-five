using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OrbData
{
    public string uid;
    public Vector3 position;
    // You can add other fields such as rotation, orb type, etc.
}

public class GameCheckpoint
{
    public Vector3 playerPosition;
    public List<string> pickedUpOrbUIDs; // Orbs in the stack, referenced by UID.
    public List<OrbData> remainingOrbs;  // Orbs not picked up, with their positions.
}

public class ButtonScript : MonoBehaviour
{
    public DoorScript door;
    // Assign the Door GameObject (with DoorScript) in the Inspector.
    public GameObject instructionText;
    public GameObject Panel2;
    public GameObject globalLightObject;
    private Light2D globalLight;
    public PlayerController player;       // Assign the Player GameObject (with PlayerController) in the Inspector.
    public Color pressedColor = Color.gray;  // Color to change to when the button is pressed.
    private float newIntensity = 1f;
    private bool activated = false;
    private SpriteRenderer sr;
    public GameAnalytics gameAnalytics;
    private Stack<GameObject> checkpointStack;
    private List<string> pickedUpOrbUIDs = new List<string>();
    //private Stack<GameObject> remainingOrbs;
    private List<OrbData> remainingOrbData = new List<OrbData>();

    void Start()
    {
        // Get the SpriteRenderer component from the button.
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated && collision.CompareTag("Player"))
        {
            activated = true;

            gameAnalytics.StartReturnJourney();

            // Change the button color to indicate it has been pressed.
            if (sr != null)
            {
                sr.color = pressedColor;
            }
            if (globalLightObject != null) { 
            globalLight = globalLightObject.GetComponent<Light2D>();
            globalLight.intensity = newIntensity;
            }
            // Show text if it exists
            if (instructionText != null) instructionText.SetActive(true);
            if (Panel2 != null) Panel2.SetActive(true);

            
            // Trigger the door opening and player transformation.
            door.OpenDoor();
            player.TransformToHuman();

            saveCheckpoint();
        }
    }

    public void saveCheckpoint()
    {
        checkpointStack = player.getStack();
        for (int i = 0; i < checkpointStack.Count; i++)
        {
            OrbClass orbComponent = checkpointStack.ElementAt(i).GetComponent<OrbClass>();
            pickedUpOrbUIDs.Add(orbComponent.uniqueID);

        }

        GameObject[] blueOrbs = GameObject.FindGameObjectsWithTag("BlueOrb");
        GameObject[] yellowOrbs = GameObject.FindGameObjectsWithTag("YellowOrb");
      
        foreach (GameObject orb in blueOrbs)
        {
            OrbClass orbComponent = orb.GetComponent<OrbClass>();
            if (orbComponent != null)
            {
                // Check if this orb is not already picked up.
                if (!pickedUpOrbUIDs.Contains(orbComponent.uniqueID))
                {
                    OrbData data = new OrbData();
                    data.uid = orbComponent.uniqueID;
                    data.position = orb.transform.position;
                    remainingOrbData.Add(data);
                }
            }
        }

        foreach (GameObject orb in yellowOrbs)
        {
            OrbClass orbComponent = orb.GetComponent<OrbClass>();
            if (orbComponent != null)
            {
                if (!pickedUpOrbUIDs.Contains(orbComponent.uniqueID))
                {
                    OrbData data = new OrbData();
                    data.uid = orbComponent.uniqueID;
                    data.position = orb.transform.position;
                    remainingOrbData.Add(data);
                }
            }
        }
        
    }
}
