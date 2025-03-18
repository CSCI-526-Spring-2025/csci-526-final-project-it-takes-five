using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbClass : MonoBehaviour
{

    public string uniqueID { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        // Generate a unique identifier when the orb is created.
        uniqueID = System.Guid.NewGuid().ToString();
        Debug.Log(uniqueID);
    }
}
