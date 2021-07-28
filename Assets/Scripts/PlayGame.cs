using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayGame : MonoBehaviour
{
    GameObject PlaySaboGame;
    GameObject PlayIntelGame;
    GameObject ExitGame;
    //GameObject PlayMiningGame;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject players in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (players.GetComponent<States>().isLocalPlayer)
            {
                player = players;
            }
        }
        if (SceneManager.sceneCount > 2)
        {
            int scenes = SceneManager.sceneCount;
            for (int i = 1; i < scenes - 1; i++)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
            }
        }
        player.transform.GetChild(0).gameObject.SetActive(false);
        PlaySaboGame = transform.GetChild(0).gameObject;
        PlayIntelGame = transform.GetChild(1).gameObject;
        ExitGame = transform.GetChild(3).gameObject;
        PlaySaboGame.GetComponent<Button>().onClick.AddListener(playSabo);
        PlayIntelGame.GetComponent<Button>().onClick.AddListener(playIntel);
        ExitGame.GetComponent<Button>().onClick.AddListener(EndGame);
        States playerStates = player.GetComponent<States>();
        if (playerStates.saboCD)
        {
            PlaySaboGame.SetActive(false);
        }
        if (playerStates.intelCD)
        {
            PlayIntelGame.SetActive(false);
        }
        //PlayMiningGame.GetComponent<Button>().onClick.AddListener(playMining);
    }

    public void playIntel()
    {
        Debug.Log("clickedIntel");
        SceneManager.LoadSceneAsync("IntelGameScene", LoadSceneMode.Additive);
    }

    public void playSabo()
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
    
    public void EndGame()
    {
        player.transform.GetChild(0).gameObject.SetActive(true);
        player.transform.GetChild(0).GetComponent<AudioListener>().enabled = true;
        States playerStates = player.GetComponent<States>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerStates.finishGame = 0;
        playerStates.playingMinigame = false;
        GameObject.FindGameObjectWithTag("RoomEventSystem").GetComponent<EventSystem>().enabled = true;
        SceneManager.UnloadSceneAsync("ChooseComputerGame");
    }
}
