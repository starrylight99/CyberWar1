using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class NetworkLobbyManagerCustomised : NetworkRoomManager
{
    public bool isAttack;
    public string playerName;
    public int teamIndex;
    public int roomPlayTime = 10;
    GameObject[] players;

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
            Debug.Log("Starting Client");
            playerName = GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(0).
                GetChild(0).GetComponentInChildren<TMP_InputField>().text;
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
        Vector3 spawnPos = transform.GetChild(0).GetChild(index).position;
        if (!LobbyResources.playerTeamAttack[index])
        {
            spawnPos = new Vector3(spawnPos.x + 225, spawnPos.y, spawnPos.z);
        }
        int teamIndex = 0;
        for (int i = 0; i < index; i++)
        {
            if (LobbyResources.playerTeamAttack[i] == LobbyResources.playerTeamAttack[index])
            {
                teamIndex += 1;
            }
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
        Debug.Log(player.GetComponent<States>().spawnPos);
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
            NetworkClient.localPlayer.gameObject.transform.Find("Local Camera").GetComponent<Camera>().enabled = true;
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

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Starting Server");
    }

}
