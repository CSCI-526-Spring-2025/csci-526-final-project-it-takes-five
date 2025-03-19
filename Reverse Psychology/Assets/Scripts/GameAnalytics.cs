using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;
using System.Collections;

public class GameAnalytics : MonoBehaviour
{
    public TextMeshProUGUI timerText; // UI Text to display timer
    private float leftToRightTime;
    private float rightToLeftTime;
    private float totalTime;
    private float levelTimer;
    private float levelStartTime;
    private string levelNumber;

    //private int playerDeaths;

    private float endTime;

    private bool pauseTime;

    //private DatabaseReference dbReference;
    private bool isReturning;
    //private FirebaseApp firebaseApp;


    void Start()
    {
        ResetMetrics();
        GameObject textObject = GameObject.Find("CurrentLevel");
        pauseTime = false;
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

    void Update()
    {
        if (!pauseTime) {
            endTime = Time.time;
        }
        float elapsedTime = endTime - levelTimer;
        timerText.text = "Time: " + elapsedTime.ToString("F2") + "s";
    }

    // Event handlers
    public void OnPlayerRestart()
    {
        Debug.Log("Restart data send");
        StartCoroutine(SaveLevelRestartAnalyticsData());

    }

    public void EndLevel()
    {
        if (isReturning)
        {
            pauseTime = true;
            Debug.Log("journey ended");
            rightToLeftTime = Time.time - levelStartTime;
            totalTime = leftToRightTime + rightToLeftTime;
            
            StartCoroutine(SaveAnalyticsData());
        }
    }

    public void EndLevelDeath()
    {
        if (isReturning)
        {
            pauseTime = true;
            Debug.Log("journey ended");
            //rightToLeftTime = Time.time - levelStartTime;
            //totalTime = leftToRightTime + rightToLeftTime;
            StartCoroutine(SaveLevelDeathAnalyticsData());
        }
    }

    public void EndAbility(string type, bool success)
    {
        if (isReturning) {
            StartCoroutine(SaveAbilityAnalyticsData(type, success));
        }
    }

    // Data posting functions

    private IEnumerator SaveLevelDeathAnalyticsData()
    {
        Debug.Log("Level Death Analytics data sending!!");
        string timestamp = DateTime.UtcNow.ToString("_yyyy-MM-dd-HH-mm-ss");
        string userId = SystemInfo.deviceUniqueIdentifier;

        LevelAnalyticsData data = new LevelAnalyticsData(levelNumber);
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

        LevelAnalyticsData data = new LevelAnalyticsData(levelNumber);
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

    private IEnumerator SaveAbilityAnalyticsData(string type, bool success)
    {
        Debug.Log("Ability Analytics data sending!!");
        string timestamp = DateTime.UtcNow.ToString("_yyyy-MM-dd-HH-mm-ss");
        // string userId = SystemInfo.deviceUniqueIdentifier;

        AbilityAnalyticsData data = new AbilityAnalyticsData(levelNumber, type, success, timestamp);
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

[Serializable]
public class LevelAnalyticsData
{
    public string levelNumber;

    public LevelAnalyticsData(string level)
    {
        levelNumber = level;
    }
}

[Serializable]
public class AbilityAnalyticsData
{
    public string level;
    public string ability;
    public bool success;
    public string timestamp;


    public AbilityAnalyticsData(string level, string ability, bool success, string timestamp)
    {
        this.level = level;
        this.ability = ability;
        this.success = success;
        this.timestamp = timestamp;
    }
}