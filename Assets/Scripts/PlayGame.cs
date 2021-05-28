using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayGame : MonoBehaviour
{
    GameObject PlaySaboGame;
    GameObject PlayIntelGame;
    // Start is called before the first frame update
    void Start()
    {
        PlaySaboGame = transform.GetChild(0).gameObject;
        PlayIntelGame = transform.GetChild(1).gameObject;
        PlaySaboGame.GetComponent<Button>().onClick.AddListener(playSabo);
        PlayIntelGame.GetComponent<Button>().onClick.AddListener(playIntel);
    }

    private void playIntel()
    {
        Debug.Log("clickedIntel");
        SceneManager.LoadScene("IntelGameScene");
    }

    private void playSabo()
    {
        Debug.Log("clickedSabo");
        SceneManager.LoadScene("SabotageGameScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
