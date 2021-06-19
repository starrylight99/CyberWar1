using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;

public class NetworkRoomPlayerScript : NetworkRoomPlayer
{
    GameObject readyScene;
    GameObject lobby;
    GameObject[] players;
    bool hasChosenCharacter = false;
    [SyncVar]
    public int spriteIndex;
    public string displayName { get; set; } = null;


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("starting player");
        displayName = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkLobbyManagerCustomised>().playerName;
        CmdAddName(displayName);
        CmdSendServerUpdateName();
        CmdSendServerUpdateSprite();
        CmdShowReadyStateUser(false);
        lobby = GameObject.FindGameObjectWithTag("Lobby");
        readyScene = lobby.transform.GetChild(1).gameObject;
        players = Resources.LoadAll<GameObject>("SpawnablePrefabs/");
        StartCoroutine("enableChooseCharacter");
        readyScene.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(ReadyUp);
        readyScene.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(Quit);
    }

    private IEnumerator enableChooseCharacter()
    {
        yield return new WaitForSeconds(0.5f);
        Button chooseChar;
        if (readyScene.transform.GetChild(index + 3).GetChild(1).gameObject.GetComponent<Button>() == null)
        {
            chooseChar = readyScene.transform.GetChild(index + 3).GetChild(1).gameObject.AddComponent<Button>();

        }
        else
        {
            chooseChar = readyScene.transform.GetChild(index + 3).GetChild(1).gameObject.GetComponent<Button>();
            readyScene.transform.GetChild(index + 3).GetChild(1).gameObject.GetComponent<Image>().sprite =
                Resources.Load<Sprite>("shadedDark48");
        }
        chooseChar.onClick.AddListener(ChooseCharacter);
        readyScene.transform.GetChild(index + 3).GetChild(1).GetComponent<Image>().color =
            new Color(255f, 255f, 255f, 255f);
    }

    private void Quit()
    {
        if (index == 0)
        {
            GameObject.FindGameObjectWithTag("NetworkManager").
                GetComponent<NetworkLobbyManagerCustomised>().StopHost(); //for now?
        }
        else
        {
            if (isLocalPlayer)
            {
                giveTime();
                CmdRemoveEntry(index);
                StartCoroutine("removeClient");
            }
        }
    }

    [Command]
    void giveTime()
    {
        Debug.Log(Time.time);
    }

    private IEnumerator removeClient()
    {
        yield return new WaitForSeconds(3f);
        if (isLocalPlayer)
        {
            GameObject.FindGameObjectWithTag("NetworkManager").
                GetComponent<NetworkLobbyManagerCustomised>().StopClient();
        }
    }

    private void ReadyUp()
    {
        if (hasChosenCharacter)
        {
            if (index == 0)
            {
                lobby.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                lobby.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                lobby.transform.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(StartGame);
            }
            else
            {
                readyToBegin = true;
                CmdChangeReadyState(readyToBegin);
                lobby.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                lobby.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                lobby.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(NotReady);
            }
            CmdShowReadyStateUser(true);
        }
        else
        {
            Debug.Log("choose character first");
        }
    }

    private void NotReady()
    {
        readyToBegin = false;
        CmdChangeReadyState(readyToBegin);
        CmdShowReadyStateUser(false);
        lobby.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        lobby.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
    }

    private void StartGame()
    {
        readyToBegin = true;
        CmdChangeReadyState(readyToBegin);
        if (GameObject.FindGameObjectWithTag("NetworkManager").
            GetComponent<NetworkLobbyManagerCustomised>().allPlayersReady == false)
        {
            Debug.Log("Not Every Player is Ready");
            readyToBegin = false;
            CmdChangeReadyState(readyToBegin);
            //uncomment if want host to ready again before starting game
            //GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            //GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        }
    }

    private void ChooseCharacter()
    {
        GameObject chooseChar = lobby.transform.GetChild(2).gameObject;
        chooseChar.SetActive(true);
        for (int i = 0; i < players.Length; i++)
        {
            GameObject player = players[i];
            chooseChar.transform.GetChild(0).GetChild(i).GetComponent<Button>().
                onClick.AddListener(() =>
                {
                    ChooseThis(player);
                });
        }
        chooseChar.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(Close);
    }

    private void Close()
    {
        CmdSendServerUpdateSprite();
        lobby.transform.GetChild(2).gameObject.SetActive(false);
    }

    public void ChooseThis(GameObject character)
    {
        int i = 0;
        foreach (GameObject possible in players)
        {
            if (possible.Equals(character)) { break; }
            else { i++; }
        }
        hasChosenCharacter = true;
        CmdAddSprite(i, index);
    }

    [Command]
    public void CmdShowReadyStateUser(bool isReady)
    {
        if (isReady) { LobbyResources.playerReadyState[index] = true; }
        else { LobbyResources.playerReadyState[index] = false; }
        updateReadyStateClientRpc(LobbyResources.playerReadyState);
    }

    [Command]
    public void CmdShowReadyStateUserNoChange()
    {
        updateReadyStateClientRpc(LobbyResources.playerReadyState);
    }

    [Command]
    public void CmdSendServerUpdateName()
    {
        Debug.Log("playerNames Count when giving: " + LobbyResources.playerNames.Count.ToString());
        updateNamesClientRpc(LobbyResources.playerNames);
    }

    [Command]
    public void CmdSendServerUpdateSprite()
    {
        updateSpritesClientRpc(LobbyResources.playerSprites, LobbyResources.playerIndexforSprite, LobbyResources.playerNames);
    }

    [Command]
    public void CmdAddName(string name)
    {
        if (isServer)
        {
            LobbyResources.playerNames.Add(name);
            LobbyResources.playerReadyState.Add(false);
        }
    }

    [Command]
    public void CmdAddSprite(int sprite, int index)
    {
        spriteIndex = sprite;
        if (isServer)
        {
            bool isPresent = false;
            for (int i = 0; i < LobbyResources.playerSprites.Count; i++)
            {
                if (LobbyResources.playerIndexforSprite[i] == index)
                {
                    LobbyResources.playerSprites[i] = sprite;
                    isPresent = false;
                }
            }
            if (!isPresent)
            {
                LobbyResources.playerSprites.Add(sprite);
                LobbyResources.playerIndexforSprite.Add(index);
            }
        }
    }

    [Command]
    void CmdRemoveEntry(int index)
    {
        LobbyResources.playerNames.RemoveAt(index);
        LobbyResources.playerReadyState.RemoveAt(index);
        int removeIndex = -1;
        for (int i = 0; i < LobbyResources.playerIndexforSprite.Count; i++)
        {
            if (LobbyResources.playerIndexforSprite[i] == index)
            {
                removeIndex = i;
            }
            else if (LobbyResources.playerIndexforSprite[i] > index)
            {
                LobbyResources.playerIndexforSprite[i]--;
            }
        }
        if (removeIndex != -1)
        {
            LobbyResources.playerIndexforSprite.RemoveAt(removeIndex);
            LobbyResources.playerSprites.RemoveAt(removeIndex);
            updateSpritesClientRpc(LobbyResources.playerSprites, LobbyResources.playerIndexforSprite, LobbyResources.playerNames);
        }
        updateNamesClientRpc(LobbyResources.playerNames);
        updateReadyStateClientRpc(LobbyResources.playerReadyState);
        Debug.Log(Time.time);
    }

    [ClientRpc]
    public void updateReadyStateClientRpc(List<bool> playerReadyState)
    {
        readyScene = GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(1).gameObject;
        for (int i = 0; i < playerReadyState.Count; i++)
        {
            if (playerReadyState[i])
            {
                readyScene.transform.GetChild(i + 3).GetChild(0).
                    GetComponent<TextMeshProUGUI>().color = new Color(57 / 255.0f, 1f, 20 / 255.0f);
            }
            else
            {
                readyScene.transform.GetChild(i + 3).GetChild(0).
                    GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
            }
        }
        for (int i = playerReadyState.Count; i < 3; i++)
        {
            readyScene.transform.GetChild(i + 3).GetChild(0).
                    GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
        }

    }

    [ClientRpc]
    public void updateSpritesClientRpc(List<int> playerSprites, List<int> playerIndex, List<string> playerNames)
    {
        readyScene = GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(1).gameObject;
        players = Resources.LoadAll<GameObject>("SpawnablePrefabs/");
        for (int i = 0; i < playerSprites.Count; i++)
        {
            readyScene.transform.GetChild(playerIndex[i] + 3).GetChild(1).GetComponent<RectTransform>().sizeDelta =
                new Vector2(110, 140);
            readyScene.transform.GetChild(playerIndex[i] + 3).GetChild(1).GetComponent<Image>().sprite =
                players[playerSprites[i]].GetComponent<SpriteRenderer>().sprite;
            readyScene.transform.GetChild(playerIndex[i] + 3).GetChild(1).GetComponent<Image>().color =
                new Color(255f, 255f, 255f, 255f);
        }
        for (int i = playerSprites.Count; i < 3; i++)
        {
            if (i >= playerNames.Count)
            {
                readyScene.transform.GetChild(i + 3).GetChild(1).GetComponent<RectTransform>().sizeDelta =
                new Vector2(60, 60);
                readyScene.transform.GetChild(i + 3).GetChild(1).GetComponent<Image>().color =
                    new Color(255f, 255f, 255f, 0f);
            }
            
        }
    }

    [ClientRpc]
    public void updateNamesClientRpc(List<string> playerNames)
    {
        GameObject readyScene = GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(1).gameObject;
        for (int i = 0; i < playerNames.Count; i++)
        {
            readyScene.transform.GetChild(i + 3).GetChild(0).GetComponent<TextMeshProUGUI>().
                SetText(playerNames[i]);
        }
        for (int i = playerNames.Count; i < 3; i++)
        {
            readyScene.transform.GetChild(i + 3).GetChild(0).GetComponent<TextMeshProUGUI>().
                SetText("");
        }
    }

}
