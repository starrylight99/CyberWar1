using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using TMPro;


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

}