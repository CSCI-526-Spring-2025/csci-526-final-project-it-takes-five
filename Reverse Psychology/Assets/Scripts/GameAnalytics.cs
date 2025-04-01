using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;
using System.Collections;

public class GameAnalytics : MonoBehaviour
{
    public bool debug = false;

    public TextMeshProUGUI timerText; // UI Text to display timer
    private float leftToRightTime;
    private float rightToLeftTime;
    private float totalTime;
    private float levelTimer;
    private float levelStartTime;
    private float endTime;
    private bool pauseTime;
    private bool sendStartFlag;

    private string levelNumber;
    private bool isReturning;

    private int width;
    private int height;

    // Reset metrics, get level number, get width and height, start timer
    void Start()
    {
        ResetMetrics();
        // Get level number
        GameObject textObject = GameObject.Find("CurrentLevel");
        pauseTime = false;
        sendStartFlag = false;
        if (textObject != null)
        {
            TextMeshProUGUI textElement = textObject.GetComponent<TextMeshProUGUI>();
            if (textElement != null)
            {
                levelNumber = textElement.text;
                Debug.Log("Text content: " + textElement.text);
            }
        }
        else
        {
            Debug.LogError("Text GameObject not found in the scene.");
        }

        levelStartTime = levelTimer = Time.time;

        // Get width and height

        //dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        //FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        //{
            
        //    Firebase.DependencyStatus dependencyStatus = task.Result;
            
        //    if (dependencyStatus == Firebase.DependencyStatus.Available)
        //    {
                
        //        firebaseApp = FirebaseApp.DefaultInstance;
        //        //Debug.Log("Firebase Initialization started");
        //        //firebaseApp.Options.DatabaseUrl = new System.Uri("https://gameanalytics-its-default-rtdb.firebaseio.com/");
                
        //        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        //        Debug.Log("Firebase Initialized Successfully");
        //    }
        //    else
        //    {
        //        Debug.LogError($"Firebase dependencies error: {dependencyStatus}");
        //    }
        //});
    }

    // Update timer
    void Update()
    {
        if (!pauseTime) {
            endTime = Time.time;
        }
        float elapsedTime = endTime - levelTimer;
        timerText.text = "Time: " + elapsedTime.ToString("F2") + "s";

       
        if(!sendStartFlag && Time.time - levelTimer > 5f)
        {
            OnPlayerStart();
            sendStartFlag = true;
        }
    }

    public void OnPlayerStart()
    {
        if (debug) return;
        Debug.Log("Start data send");
        StartCoroutine(SaveLevelStartAnalyticsData());

    }

    private IEnumerator SaveLevelStartAnalyticsData()
    {
        Debug.Log("Level Start Analytics data sending!!");
        string timestamp = DateTime.UtcNow.ToString("_yyyy-MM-dd-HH-mm-ss");
        string userId = SystemInfo.deviceUniqueIdentifier;

        LevelAnalyticsData data = new LevelAnalyticsData(levelNumber, 0.0f, 0.0f);
        string json = JsonUtility.ToJson(data);


        Debug.Log($"Level {levelNumber} Start");
        string URL = "https://gameanalytics-its-default-rtdb.firebaseio.com/start/";
        //string key = "Level_" + userId + timestamp;
        string key = timestamp;

        //string databaseSecret = "AIzaSyBanWvgz3YKrMyGBrmfcer1Sub0qxcwPW0";  // Replace with your actual secret key


        using (var uwr = new UnityWebRequest(URL + key + ".json", "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            using UploadHandlerRaw uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.uploadHandler = uploadHandler;
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.disposeUploadHandlerOnDispose = true;
            uwr.disposeDownloadHandlerOnDispose = true;
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.timeout = 5;
            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
                Debug.Log("Error While Sending:" + uwr.error + " TimeStamp:" + timestamp);
            else
                Debug.Log("Data Received:" + uwr.downloadHandler.text + "TimeStamp: " + timestamp);
        }
        yield return new WaitForSeconds(1f);  // Example of delay
        Debug.Log("Level Analytics data saved.");
    }


    // Event handlers for restart, level end, and ability use. Call coroutines to send data

    // Send Level Data when player restarts: Level, Timestamp, dummy position to /restart
    // Used for num restarts graph
    public void OnPlayerRestart()
    {
        if(debug) return;
        Debug.Log("Restart data send");
        StartCoroutine(SaveLevelRestartAnalyticsData());

    }
 
    // Send Level Data when player dies: Level, Timestamp, position to /death
    // Used for death heatmap
    public void EndLevelDeath(float x, float y)
    {
        if(debug) return;
        if (isReturning)
        {
            pauseTime = true;
            Debug.Log("Death data send");
            StartCoroutine(SaveLevelDeathAnalyticsData(x, y));
        }
    }

    // Send Analytics Data when player completes level: Level, Left to Right Time, Right to Left Time, Total Time to /timedata
    // Used for completion time box plots
    public void EndLevel()
    {
        if(debug) return;
        if (isReturning)
        {
            pauseTime = true;
            Debug.Log("Level completed");
            rightToLeftTime = Time.time - levelStartTime;
            totalTime = leftToRightTime + rightToLeftTime;
            
            StartCoroutine(SaveAnalyticsData());
        }
    }

    // Send Ability Data when player uses ability: Level, Ability, Success, Timestamp, Position to /abilitydata
    // Used for ability heatmap
    public void EndAbility(string type, bool success, float x, float y)
    {
        if(debug) return;
        if (isReturning) {
            StartCoroutine(SaveAbilityAnalyticsData(type, success, x, y));
        }
    }

    // Data posting functions ____________________________________________________________

    private IEnumerator SaveLevelDeathAnalyticsData(float x, float y)
    {
        Debug.Log("Level Death Analytics data sending!!");
        string timestamp = DateTime.UtcNow.ToString("_yyyy-MM-dd-HH-mm-ss");
        string userId = SystemInfo.deviceUniqueIdentifier;

        LevelAnalyticsData data = new LevelAnalyticsData(levelNumber, x, y);
        string json = JsonUtility.ToJson(data);


        
        string URL = "https://gameanalytics-its-default-rtdb.firebaseio.com/death/";
        //string key = "Level_" + userId + timestamp;
        string key = timestamp;

        //string databaseSecret = "AIzaSyBanWvgz3YKrMyGBrmfcer1Sub0qxcwPW0";  // Replace with your actual secret key


        using (var uwr = new UnityWebRequest(URL + key + ".json", "POST"))
        {
            Debug.Log($"Level {levelNumber} Death");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            using UploadHandlerRaw uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.uploadHandler = uploadHandler;
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.disposeUploadHandlerOnDispose = true;
            uwr.disposeDownloadHandlerOnDispose = true;
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.timeout = 5;
            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
                Debug.Log("Error While Sending:" + uwr.error + " TimeStamp:" + timestamp);
            else
                Debug.Log("Data Received:" + uwr.downloadHandler.text + "TimeStamp: " + timestamp);
        }
        yield return new WaitForSeconds(1f);  // Example of delay
        Debug.Log("Level Analytics data saved.");
    }

    private IEnumerator SaveLevelRestartAnalyticsData()
    {
        Debug.Log("Level Restart Analytics data sending!!");
        string timestamp = DateTime.UtcNow.ToString("_yyyy-MM-dd-HH-mm-ss");
        string userId = SystemInfo.deviceUniqueIdentifier;

        LevelAnalyticsData data = new LevelAnalyticsData(levelNumber, 0.0f, 0.0f);
        string json = JsonUtility.ToJson(data);


        Debug.Log($"Level {levelNumber} restart");
        string URL = "https://gameanalytics-its-default-rtdb.firebaseio.com/restart/";
        //string key = "Level_" + userId + timestamp;
        string key = timestamp;

        //string databaseSecret = "AIzaSyBanWvgz3YKrMyGBrmfcer1Sub0qxcwPW0";  // Replace with your actual secret key


        using (var uwr = new UnityWebRequest(URL + key + ".json", "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            using UploadHandlerRaw uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.uploadHandler = uploadHandler;
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.disposeUploadHandlerOnDispose = true;
            uwr.disposeDownloadHandlerOnDispose = true;
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.timeout = 5;
            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
                Debug.Log("Error While Sending:" + uwr.error + " TimeStamp:" + timestamp);
            else
                Debug.Log("Data Received:" + uwr.downloadHandler.text + "TimeStamp: " + timestamp);
        }
        yield return new WaitForSeconds(1f);  // Example of delay
        Debug.Log("Level Analytics data saved.");
    }

    private IEnumerator SaveAnalyticsData()
    {
        Debug.Log("Analytics data sending!!");
        string timestamp = DateTime.UtcNow.ToString("_yyyy-MM-dd-HH-mm-ss");
        // string userId = SystemInfo.deviceUniqueIdentifier;

        AnalyticsData data = new AnalyticsData(levelNumber, leftToRightTime, rightToLeftTime, totalTime, timestamp);
        string json = JsonUtility.ToJson(data);

        //dbReference.Child("analytics").Child(userId).Child(timestamp).SetRawJsonValueAsync(json);
        //json ={ }

        Debug.Log($"Level {levelNumber} Completion Time: Left to Right = {leftToRightTime} sec, Right to Left = {rightToLeftTime} sec, Total = {totalTime} sec");
        //Debug.Log($"Player Deaths: {playerDeaths}");
        string URL = "https://gameanalytics-its-default-rtdb.firebaseio.com/timedata/";
        //string key = userId+timestamp;
        string key = timestamp;


        string databaseSecret = "AIzaSyBanWvgz3YKrMyGBrmfcer1Sub0qxcwPW0";  // Replace with your actual secret key


        using (var uwr = new UnityWebRequest(URL + key + ".json", "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            using UploadHandlerRaw uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.uploadHandler = uploadHandler;
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.disposeUploadHandlerOnDispose = true;
            uwr.disposeDownloadHandlerOnDispose = true;
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.timeout = 5;
            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();

            //string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            if (uwr.result != UnityWebRequest.Result.Success)
                Debug.Log("Error While Sending:" + uwr.error + " TimeStamp:" + timestamp);
            else
                Debug.Log("Data Received:" + uwr.downloadHandler.text + "TimeStamp: " + timestamp);
        }
        yield return new WaitForSeconds(1f);  // Example of delay
        Debug.Log("Analytics data saved.");
    }

    private IEnumerator SaveAbilityAnalyticsData(string type, bool success, float x, float y)
    {
        Debug.Log("Ability Analytics data sending!!");
        string timestamp = DateTime.UtcNow.ToString("_yyyy-MM-dd-HH-mm-ss");
        // string userId = SystemInfo.deviceUniqueIdentifier;

        AbilityAnalyticsData data = new AbilityAnalyticsData(levelNumber, type, success, timestamp, x, y);
        string json = JsonUtility.ToJson(data);

        Debug.Log($"Level {levelNumber} Ability Used: {type}, Success = {success}");
        string URL = "https://gameanalytics-its-default-rtdb.firebaseio.com/abilitydata/";
        string key = timestamp;

        string databaseSecret = "AIzaSyBanWvgz3YKrMyGBrmfcer1Sub0qxcwPW0";  // Replace with your actual secret key

        using (var uwr = new UnityWebRequest(URL + key + ".json", "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            using UploadHandlerRaw uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.uploadHandler = uploadHandler;
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.disposeUploadHandlerOnDispose = true;
            uwr.disposeDownloadHandlerOnDispose = true;
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.timeout = 5;
            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();

            //string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            if (uwr.result != UnityWebRequest.Result.Success)
                Debug.Log("Error While Sending:" + uwr.error + " TimeStamp:" + timestamp);
            else
                Debug.Log("Data Received:" + uwr.downloadHandler.text + "TimeStamp: " + timestamp);
        }
        yield return new WaitForSeconds(1f);  // Example of delay
        Debug.Log("Ability Analytics data saved.");
    }

    // Helper functions
    void ResetMetrics()
    {
        leftToRightTime = 0;
        rightToLeftTime = 0;
        totalTime = 0;
        //playerDeaths = 0;
        isReturning = false;
    }

    public void StartReturnJourney()
    {
        Debug.Log("return journey started");
        leftToRightTime = Time.time - levelStartTime;
        isReturning = true;
        levelStartTime = Time.time;
    }
    
}

// Data templates

[Serializable]
public class AnalyticsData
{
    public string levelNumber;
    public float leftToRightTime;
    public float rightToLeftTime;
    public float totalTime;
    //public int playerDeaths;
    public string timestamp;

    public AnalyticsData(string level, float ltr, float rtl, float total, 
        //int deaths, 
        string time)
    {
        levelNumber = level;
        leftToRightTime = ltr;
        rightToLeftTime = rtl;
        totalTime = total;
        //playerDeaths = deaths;
        timestamp = time;
    }
}

// Level Analytics: Level, Timestamp. Records end of a level (for death or restart)
[Serializable]
public class LevelAnalyticsData
{
    public string levelNumber;
    public float x;
    public float y;

    public LevelAnalyticsData(string level, float x, float y)
    {
        levelNumber = level;
        this.x = x;
        this.y = y;
    }
}

// Ability Analytics: Level, Ability, Success, Timestamp
[Serializable]
public class AbilityAnalyticsData
{
    public string level;
    public string ability;
    public bool success;
    public string timestamp;
    public float x;
    public float y;


    public AbilityAnalyticsData(string level, string ability, bool success, string timestamp, float x, float y)
    {
        this.level = level;
        this.ability = ability;
        this.success = success;
        this.timestamp = timestamp;
        this.x = x;
        this.y = y;
    }
}

