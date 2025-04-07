using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RemainingOrbData
{
    public string uid;
    public Vector3 position;
    // You can add other fields such as rotation, orb type, etc.
}

public class StackOrbData
{
    public string uid;
    public Color color;
}



public class ButtonScript : MonoBehaviour
{
    public class GameCheckpoint
    {
        public Vector3 playerPosition;
        public List<StackOrbData> pickedUpOrbs; // Orbs in the stack, referenced by UID.
        public List<RemainingOrbData> remainingOrbs;  // Orbs not picked up, with their positions.
    }
    public DoorScript door;
    // Assign the Door GameObject (with DoorScript) in the Inspector.
    public GameObject instructionText;
    public GameObject Panel1;
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
    private List<StackOrbData> pickedUpOrb = new List<StackOrbData>();
    private List<string> pickedUpOrbUIDs = new List<string>();
    private List<RemainingOrbData> remainingOrbData = new List<RemainingOrbData>();
    public GameCheckpoint gameCheckpoint = new GameCheckpoint();
    public List<GameObject> allOrbs = new List<GameObject>();


    public GameObject checkpointText;

    public bool checkpointCreated = false;

    void Start()
    {
        if (checkpointText != null) checkpointText.SetActive(false);
        // Get the SpriteRenderer component from the button.
        allOrbs = GameObject.FindGameObjectsWithTag("BlueOrb")
                                        .Union(GameObject.FindGameObjectsWithTag("YellowOrb")).ToList();
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
            if (Panel1 != null) Panel1.SetActive(false);

            
            // Trigger the door opening and player transformation.
            door.OpenDoor();
            player.TransformToHuman();

            saveCheckpoint();
        }
    }

    public void saveCheckpoint()
    {
        Debug.Log("in checkpoint saved");

        checkpointStack = player.getStack();
        for (int i = 0; i < checkpointStack.Count; i++)
        {
            StackOrbData data = new StackOrbData();
            string uid = checkpointStack.ElementAt(i).GetComponent<OrbClass>().uniqueID;
            data.uid = uid;
            data.color = checkpointStack.ElementAt(i).GetComponent<SpriteRenderer>().color;
            //checkpointStack.ElementAt((int)i).GetComponent<Sprit>
            pickedUpOrbUIDs.Add(uid);
            pickedUpOrb.Add(data);
            Debug.Log("pickedup orb " + data.uid +" "+ data.color);

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
                    RemainingOrbData data = new RemainingOrbData();
                    data.uid = orbComponent.uniqueID;
                    data.position = orb.transform.position;
                    remainingOrbData.Add(data);
                    Debug.Log("remining blue orb " + data.uid);

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
                    RemainingOrbData data = new RemainingOrbData();
                    data.uid = orbComponent.uniqueID;
                    data.position = orb.transform.position;
                    remainingOrbData.Add(data);
                }
            }
        }

        gameCheckpoint.remainingOrbs = remainingOrbData;
        gameCheckpoint.pickedUpOrbs = pickedUpOrb;
        gameCheckpoint.playerPosition = player.transform.position;
        Debug.Log("checkpoint saved");
        checkpointCreated = true;
        if (checkpointText != null)
        {
            StartCoroutine(ShowAndHideCheckpoint());
        }

    }
    private IEnumerator ShowAndHideCheckpoint()
    {
        checkpointText.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        checkpointText.SetActive(false);
    }
    public void LoadCheckpoint()
    {
        Debug.Log("in restart");
        //if (PlayerPrefs.HasKey("GameCheckpoint"))
        //{
            //string json = PlayerPrefs.GetString("GameCheckpoint");
            //GameCheckpoint checkpoint = JsonUtility.FromJson<GameCheckpoint>(json);
            player.transform.position = gameCheckpoint.playerPosition;

            // Rebuild the orb stack:
            player.orbStack.Clear();
            clearFloorOrbs();
            for (int i = gameCheckpoint.pickedUpOrbs.Count - 1; i >= 0; i--)
            {
                StackOrbData data = gameCheckpoint.pickedUpOrbs.ElementAt(i);
                Debug.Log("in stack "+data.uid);
                GameObject orb = FindOrbByUID(data.uid);
                if (orb != null)
                {
                    Debug.Log("in stack cp ", orb);
                    player.orbStack.Push(orb);
                }
            }
            player.UpdateOrbUI();

            // Restore remaining orbs.
            foreach (RemainingOrbData data in gameCheckpoint.remainingOrbs)
            {
                Debug.Log("remining orb " + data.uid);
                GameObject orb = FindOrbByUID(data.uid);
                if (orb != null)
                {
                Debug.Log("remaining cp ", orb);
                orb.transform.position = data.position;
                    // Optionally, re-enable orb if it was deactivated.
                    orb.SetActive(true);
                }
                else
                {
                    // Alternatively, instantiate the orb if it doesn't exist.
                    // You would need a prefab mapping to do this.
                }
            }

            //Debug.Log("Checkpoint loaded: " + json);
        //}
        // else
        // {
        //    Debug.Log("No checkpoint found.");
        // }
    }

    private GameObject FindOrbByUID(string uid)
    {
        // This is one possible implementation:
        //GameObject[] allOrbs = GameObject.FindGameObjectsWithTag("BlueOrb");
        foreach (GameObject orb in allOrbs)
        {
            OrbClass orbComponent = orb.GetComponent<OrbClass>();
            Debug.Log("find uid orb " + orbComponent.uniqueID);
            if (orbComponent != null && orbComponent.uniqueID == uid)
            {
                return orb;
            }
        }

        //allOrbs = GameObject.FindGameObjectsWithTag("YellowOrb");
        //foreach (GameObject orb in allOrbs)
        //{
        //    OrbClass orbComponent = orb.GetComponent<OrbClass>();
        //    Debug.Log("yellow orb " + orbComponent.uniqueID);
        //    if (orbComponent != null && orbComponent.uniqueID == uid)
        //    {
        //        return orb;
        //    }
        //}
        return null;
    }

    private void clearFloorOrbs()
    {
        List<GameObject> allOrbs = GameObject.FindGameObjectsWithTag("BlueOrb").Union(GameObject.FindGameObjectsWithTag("YellowOrb")).ToList();
        foreach (GameObject orb in allOrbs) { 
            orb.SetActive(false);
        }
    }

}
