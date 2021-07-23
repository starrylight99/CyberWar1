using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Events;
using PlayFab;

public class NetworkLobbyManagerCustomised : NetworkRoomManager
{
    public Configuration configuration;
    public bool isAttack;
    public string playerName;
    public int teamIndex;
    public int roomPlayTime = 60;
    GameObject[] players;
    bool shutdown;
    public override void OnRoomStartServer()
    {
        //Get list of spawnable prefabs on start server
        base.OnRoomStartServer();
        players = Resources.LoadAll<GameObject>("SpawnablePrefabs/");
    }

    public override void OnRoomStartClient()
    {
        // When starting client, get the team selection and display name from the host/join screen and
        // will be transferred to NetworkRoomPlayerScript
        base.OnRoomStartClient();
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            playerName = GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(0).
                GetChild(0).GetComponentInChildren<TMP_InputField>().text;
            if (playerName.Length == 0)
            {
                playerName = "player#" + ((int) UnityEngine.Random.Range(1000, 9999)).ToString();
            }
            //if (LobbyResources.playerNamesAtk.Contains(playerName) ||
            //    LobbyResources.playerNamesDef.Contains(playerName))
            //{
            //    playerName += ((int) UnityEngine.Random.Range(10, 99)).ToString();
            //}
            isAttack = GameObject.FindGameObjectWithTag("Lobby").GetComponent<LobbyUI>().isAttack;
            //Switch the screen from Host/Join screen to the Ready screen
            GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(1).gameObject.SetActive(true);
        } 
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        // Called when the server prepares to spawn the player GameObjects on the Gameplay Scene (RoomScene)
        // Will configure spawn point for room and final scene, display name, start time(in case of ping) and side
        // on States script of the player GameObject
        int spriteIndex = roomPlayer.GetComponent<NetworkRoomPlayerScript>().spriteIndex;
        int index = roomPlayer.GetComponent<NetworkRoomPlayerScript>().index;
        int teamIndex = 0;
        for (int i = 0; i < index; i++)
        {
            if (LobbyResources.playerTeamAttack[i] == LobbyResources.playerTeamAttack[index])
            {
                teamIndex += 1;
            }
        }
        Vector3 spawnPos = transform.GetChild(0).GetChild(teamIndex).position;
        if (!LobbyResources.playerTeamAttack[index])
        {
            spawnPos = new Vector3(spawnPos.x + 300, spawnPos.y, spawnPos.z);
        }
        
        GameObject player = Instantiate(players[spriteIndex], spawnPos, Quaternion.identity);
        player.GetComponent<States>().startTime = LobbyResources.timer;
        if (LobbyResources.playerTeamAttack[index])
        {
            player.GetComponent<States>().displayName =
                LobbyResources.playerNamesAtk[teamIndex];
            player.GetComponent<States>().spawnPos = transform.GetChild(1).GetChild(teamIndex).position;
        }
        else
        {
            player.GetComponent<States>().displayName =
                LobbyResources.playerNamesDef[teamIndex];
            player.GetComponent<States>().spawnPos = transform.GetChild(2).GetChild(teamIndex).position;
        }
        player.GetComponent<States>().teamIndex = teamIndex;
        player.GetComponent<States>().isAttack = LobbyResources.playerTeamAttack[index];
        return player;
    }

    public override void OnRoomStopHost()
    {
        // Clear the lobby resources on stopping host
        base.OnRoomStopHost();
        refresh();
    }

    void refresh()
    {
        // Basically reinitialise every stored list in lobby resources
        LobbyResources.playerNamesAtk = new List<string>();
        LobbyResources.playerSpritesAtk = new List<int>();
        LobbyResources.playerIndexforSpriteAtk = new List<int>();
        LobbyResources.playerReadyStateAtk = new List<bool>();

        LobbyResources.playerNamesDef = new List<string>();
        LobbyResources.playerSpritesDef = new List<int>();
        LobbyResources.playerIndexforSpriteDef = new List<int>();
        LobbyResources.playerReadyStateDef = new List<bool>();

        LobbyResources.playerTeamAttack = new List<bool>();
        LobbyResources.timer = 0;
}
    public override void OnStopClient()
    {
        // When stopping client, if the active scene is in lobby now, we allow the user to join back in again
        // in other scenes, the user's game object will still be in the network but unmovable and client cannot 
        // join back in
        base.OnStopClient();
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            GameObject lobby = GameObject.FindGameObjectWithTag("Lobby");
            GameObject titleScreen = lobby.transform.GetChild(0).gameObject;
            titleScreen.SetActive(true);
            lobby.transform.GetChild(1).gameObject.SetActive(false);
            titleScreen.transform.GetChild(0).GetComponentInChildren<TMP_InputField>().interactable = true;
            titleScreen.transform.GetChild(1).GetChild(0).GetComponent<Button>().interactable = true;
            titleScreen.transform.GetChild(1).GetChild(1).GetComponent<TMP_InputField>().interactable = true;
            titleScreen.transform.GetChild(1).GetChild(2).GetComponent<Button>().interactable = true;
        }
    }

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        // When the network server changes scene to gameplay scene (RoomScene), start the countdown for the users 
        // to start playing phase 1
        base.OnRoomServerSceneChanged(sceneName);
        if (sceneName.Contains("RoomScene"))
        {
            //For Testing! Using 10s to change to final scene now
            StartCoroutine(CountdownToFinale(roomPlayTime));
        } else if (sceneName.Contains("FinalBattle")){
            //NetworkClient.localPlayer.gameObject.transform.Find("Local Camera").
            //    GetComponent<Camera>().enabled = true;
            //NetworkClient.localPlayer.gameObject.GetComponent<FinalBattleBehaviour>().enabled = true;

        }
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        if (SceneManager.GetActiveScene().name.Contains("FinalBattle"))
        {
            FinalBattle finale = GameObject.FindGameObjectWithTag("Flag").GetComponent<FinalBattle>();
            finale.isAttack = NetworkClient.localPlayer.gameObject.GetComponent<States>().isAttack;
            finale.player = NetworkClient.localPlayer.gameObject;
            NetworkClient.localPlayer.gameObject.transform.Find("Local Camera").
                    GetComponent<Camera>().enabled = true;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            int visionScore = 10 + GameResources.GetFOWAmount(NetworkClient.localPlayer.gameObject.GetComponent<States>().isAttack);

            foreach (GameObject player in players)
            {
                player.GetComponent<FinalBattleBehaviour>().enabled = true;
                if (player.GetComponent<NetworkIdentity>().isLocalPlayer || finale.isAttack == player.GetComponent<States>().isAttack){
                    GameObject mainVision = player.transform.Find("Vision").gameObject;

                    // Uncomment for variable vision
                    /* Light2D light = mainVision.transform.GetChild(0).GetComponent<Light2D>();
                    Light2D vision = mainVision.transform.GetChild(1).GetComponent<Light2D>();

                    light.pointLightOuterRadius = visionScore;
                    light.pointLightInnerRadius = visionScore/4*3;
                    vision.pointLightOuterRadius = visionScore;
                    vision.pointLightInnerRadius = visionScore/4*3; */

                    mainVision.SetActive(true);
                }
                Move playerMove = player.GetComponent<Move>();
                if (player.GetComponent<States>().isAttack)
                {
                    if (GameResources.defResourceAmount > 35)
                    {
                        playerMove.slowPercent = 0.60f;
                        playerMove.confusedDuration = 6f;
                    }
                    else if (GameResources.defResourceAmount > 20)
                    {
                        playerMove.slowPercent = 0.50f;
                        playerMove.confusedDuration = 5f;
                    }
                    else if (GameResources.defResourceAmount > 10)
                    {
                        playerMove.slowPercent = 0.40f;
                        playerMove.confusedDuration = 4f;
                    }
                    else if (GameResources.defResourceAmount > 5)
                    {
                        playerMove.slowPercent = 0.30f;
                        playerMove.confusedDuration = 3f;
                    }
                    else
                    {
                        playerMove.slowPercent = 0.20f;
                        playerMove.confusedDuration = 2f;
                    }
                }
                else
                {
                    if (GameResources.atkResourceAmount > 35)
                    {
                        playerMove.slowPercent = 0.40f;
                        playerMove.confusedDuration = 6f;
                    }
                    else if (GameResources.atkResourceAmount > 20)
                    {
                        playerMove.slowPercent = 0.50f;
                        playerMove.confusedDuration = 5f;
                    }
                    else if (GameResources.atkResourceAmount > 10)
                    {
                        playerMove.slowPercent = 0.60f;
                        playerMove.confusedDuration = 4f;
                    }
                    else if (GameResources.atkResourceAmount > 5)
                    {
                        playerMove.slowPercent = 0.70f;
                        playerMove.confusedDuration = 3f;
                    }
                    else
                    {
                        playerMove.slowPercent = 0.80f;
                        playerMove.confusedDuration = 2f;
                    }
                }
            }
            
        }
    }

    IEnumerator CountdownToFinale(int seconds)
    {
        // We use coroutine to monitor how many seconds have passed and store in lobby resources timer
        // if user joins later due to internet connection, the timer should still be synchronized 
        // but this is not tested yet if it works!
        LobbyResources.timer = 0;
        while (LobbyResources.timer < seconds)
        {
            yield return new WaitForSeconds(1);
            LobbyResources.timer++;
        }
        ServerChangeScene("FinalBattle");
    }
}