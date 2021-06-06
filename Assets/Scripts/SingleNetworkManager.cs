using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleNetworkManager : MonoBehaviour
{
    private static GameObject instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
