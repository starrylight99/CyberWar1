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
    //GameObject PlayMiningGame;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        Debug.Log(player);
        player.transform.GetChild(0).gameObject.SetActive(false);
        PlaySaboGame = transform.GetChild(0).gameObject;
        PlayIntelGame = transform.GetChild(1).gameObject;
        PlaySaboGame.GetComponent<Button>().onClick.AddListener(playSabo);
        PlayIntelGame.GetComponent<Button>().onClick.AddListener(playIntel);
        //PlayMiningGame.GetComponent<Button>().onClick.AddListener(playMining);
        Debug.Log("started");
    }

    private void playIntel()
    {
        Debug.Log("clickedIntel");
        SceneManager.LoadSceneAsync("IntelGameScene", LoadSceneMode.Additive);
    }

    private void playSabo()
    {
        Debug.Log("clickedSabo");
        SceneManager.LoadSceneAsync("SabotageGameScene", LoadSceneMode.Additive);
    }
    /* private void playMining()
    {
        Debug.Log("clicked Mining");
        SceneManager.LoadSceneAsync("MiningScene", LoadSceneMode.Additive);
    } */
    // Update is called once per frame
    void Update()
    {
        
    }
}
