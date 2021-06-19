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
    public string playerName;
    GameObject[] players;

    public override void OnRoomStartServer()
    {
        base.OnRoomStartServer();
        players = Resources.LoadAll<GameObject>("SpawnablePrefabs/");
        
    }

    public override void OnRoomStartClient()
    {
        base.OnRoomStartClient();
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            playerName = GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(0).
                GetChild(0).GetComponentInChildren<TMP_InputField>().text;
            GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(1).gameObject.SetActive(true);
        } 
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        int spriteIndex = roomPlayer.GetComponent<NetworkRoomPlayerScript>().spriteIndex;
        int index = roomPlayer.GetComponent<NetworkRoomPlayerScript>().index;
        Vector3 spawnPos = transform.GetChild(index).position;
        GameObject player = Instantiate(players[spriteIndex], spawnPos, Quaternion.identity);
        player.GetComponent<States>().displayName = LobbyResources.playerNames[roomPlayer.GetComponent<NetworkRoomPlayerScript>().index];
        //player.GetComponentInChildren<TextMeshProUGUI>().SetText(player.GetComponent<States>().displayName);
        Debug.Log(LobbyResources.playerNames[index]);
        return player;
    }

    public override void OnRoomStopHost()
    {
        base.OnRoomStopHost();
        refresh();
    }

    void refresh()
    {
        LobbyResources.playerNames = new List<string>();
        LobbyResources.playerSprites = new List<int>();
        LobbyResources.playerIndexforSprite = new List<int>();
        LobbyResources.playerReadyState = new List<bool>();
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Disconnecting");
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