using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;

public class States : NetworkBehaviour
{
    [SerializeField]
    public bool winIntelGame = false;
    [SerializeField]
    public bool winSaboGame = false;
    [SerializeField]
    public int resourceInventory = 0;
    [SyncVar]
    public string displayName;
    [SyncVar]
    public bool isAttack;
    public int startTime;
    float totalTime;
    bool timeIsRunning;
    public bool playingMinigame = false;
    public Vector3 spawnPos;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetName();
        totalTime = (float) startTime;
        timeIsRunning = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (timeIsRunning)
            {
                totalTime += Time.deltaTime;
                if ((int)totalTime > 8)
                {
                    timeIsRunning = false;
                    if (playingMinigame)
                    {
                        playingMinigame = false;
                        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
                        transform.GetChild(0).gameObject.SetActive(true);
                        transform.GetChild(0).GetComponent<AudioListener>().enabled = true;
                        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    }
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject player in players)
                    {
                        DontDestroyOnLoad(player);
                    }
                }
            }

            if (winIntelGame == true)
            {
                // to add notification on winning the minigame
                Debug.Log("Win Substitution Cipher Game");
                winIntelGame = false;
            }
            if (winSaboGame == true)
            {
                // to add notification on winning the minigame
                Debug.Log("Win Sabotage Game");
                winSaboGame = false;
            }
        }
    }
    [Command]
    void CmdSetName()
    {
        updateAllPlayerNamesClientRpc();
    }

    [ClientRpc]
    void updateAllPlayerNamesClientRpc()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in playerList)
        {
            if (player.GetComponent<States>().isAttack)
            {
                player.GetComponentInChildren<TextMeshProUGUI>().color = new Color(255/255.0f, 185/255.0f, 185/255.0f);
            }
            else
            {
                player.GetComponentInChildren<TextMeshProUGUI>().color = new Color(184/255.0f, 233/255.0f, 255/255.0f);
            }
            player.GetComponentInChildren<TextMeshProUGUI>().SetText(player.GetComponent<States>().displayName);
            
        }
    }

    
    
}
