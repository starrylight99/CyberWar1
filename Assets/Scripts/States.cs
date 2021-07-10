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
    [SyncVar]
    public int teamResources;
    [SyncVar]
    public string displayName;
    [SyncVar]
    public bool isAttack;
    public int startTime;
    float totalTime;
    bool timeIsRunning;
    public bool playingMinigame = false;
    public Vector3 spawnPos;
    RoomResources roomResources;
    GameHandler gameHandlerComponent;
    MiningAI miningAI;
    public int teamIndex;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetName();
        totalTime = (float) startTime;
        timeIsRunning = true;
        
        if (isAttack)
        {
            spawnPos = GameObject.FindGameObjectWithTag("NetworkManager").transform.
                GetChild(1).GetChild(teamIndex).position;
        }
        else
        {
            spawnPos = GameObject.FindGameObjectWithTag("NetworkManager").transform.
                GetChild(2).GetChild(teamIndex).position;
        }
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
    /* [Command]
    public void SetResourcesServer(int amount, bool isAtk){
        roomResources = GameObject.FindGameObjectWithTag("RoomResources").GetComponent<RoomResources>();
        if (isAtk) {
            roomResources.atkTeamResources = amount;
        } else {
            roomResources.defTeamResources = amount;
        }
        Debug.Log("Server atk: " + roomResources.atkTeamResources);
        Debug.Log("Server def: " + roomResources.defTeamResources);
        Debug.Log("Server resource: " + roomResources.atkTeamResources);
        SetResourcesClient(amount,isAtk);
    }

    [ClientRpc]
    public void SetResourcesClient(int amount, bool isAtk){
        //Debug.Log("Recieved SetResourcesClient");
        roomResources = GameObject.FindGameObjectWithTag("RoomResources").GetComponent<RoomResources>();
        if (isAtk) {
            roomResources.atkTeamResources = amount;
        } else {
            roomResources.defTeamResources = amount;
        }
        //Debug.Log("Client atk: " + roomResources.atkTeamResources);
        //Debug.Log("Client def: " + roomResources.defTeamResources);
        amount = this.isAttack ? roomResources.atkTeamResources : roomResources.defTeamResources;
        GameObject.FindGameObjectWithTag("UI").GetComponent<GameResourcesUI>().UpdateResource(amount);
        //Debug.Log("Client resource : " + roomResources.atkTeamResources);
    } */

    [Command]
    public void SetResourceNodeServer(int serial, bool isAtk){
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Mining");
        foreach (GameObject mining in arr)
        {
            if (mining.GetComponent<TeamTag>().isAttack == isAtk){
                miningAI = mining.transform.Find("Miner").GetComponent<MiningAI>();
            }
        }
        arr = GameObject.FindGameObjectsWithTag("GameHandler");
        foreach (GameObject gameHandler in arr)
        {
            if (gameHandler.GetComponent<TeamTag>().isAttack == isAtk){
                gameHandlerComponent = gameHandler.GetComponent<GameHandler>();
            }
        }
        miningAI.SetResourceNode(gameHandlerComponent.resourceNodeList[serial]);
        SetResourceNodeClient(serial,isAtk);
    }

    [ClientRpc]
    public void SetResourceNodeClient(int serial, bool isAtk){
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Mining");
        foreach (GameObject mining in arr)
        {
            if (mining.GetComponent<TeamTag>().isAttack == isAtk){
                miningAI = mining.transform.Find("Miner").GetComponent<MiningAI>();
            }
        }
        arr = GameObject.FindGameObjectsWithTag("GameHandler");
        foreach (GameObject gameHandler in arr)
        {
            if (gameHandler.GetComponent<TeamTag>().isAttack == isAtk){
                gameHandlerComponent = gameHandler.GetComponent<GameHandler>();
            }
        }
        miningAI.SetResourceNode(gameHandlerComponent.resourceNodeList[serial]);
    }
}
