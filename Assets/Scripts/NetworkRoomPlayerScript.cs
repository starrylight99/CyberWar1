using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class NetworkRoomPlayerScript : NetworkRoomPlayer
{
    GameObject readyScene;
    GameObject lobby;
    GameObject[] players;
    GameObject message;
    bool hasChosenCharacter = false;
    [SyncVar]
    public int spriteIndex;
    public string displayName { get; set; } = null;
    public bool isAttack;
    [SyncVar]
    public int teamIndex;
    private int _winGame = 0;
    public int winGame { 
        // Property to detect when the game is won or lost
        // bool might work but i set as int already lol
        get { return _winGame; }
        set
        {
            if (isLocalPlayer)
            {
                _winGame = value;
                if (_winGame == 1)
                {
                    // if the player wins, they will have the victory screen and clicking button will stop client/host
                    if (SceneManager.GetActiveScene().name == "FinalBattle")
                    {
                        GameObject endscreen = GameObject.FindGameObjectWithTag("UI").transform.GetChild(0).gameObject;
                        endscreen.SetActive(true);
                        GameObject.FindGameObjectWithTag("UI").transform.GetChild(1).gameObject.SetActive(false);
                        endscreen.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(Quit);
                    }
                }
                else if (_winGame == -1)
                {
                    // if the player wins, they will have the defeat screen and clicking button will stop client/host
                    if (SceneManager.GetActiveScene().name == "FinalBattle")
                    {
                        GameObject endscreen = GameObject.FindGameObjectWithTag("UI").transform.GetChild(1).gameObject;
                        endscreen.SetActive(true);
                        GameObject.FindGameObjectWithTag("UI").transform.GetChild(0).gameObject.SetActive(false);
                        endscreen.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(Quit);
                    }
                }
            }
        }
    }


    public override void OnStartLocalPlayer()
    {
        // when starting player, initialise the displayname and team side
        // Commands with Add are used to update lobby resources with the new display name and team side
        // Commands with Update/Show are used to call ClientRpcs to update all clients on the new client joining in
        // initialise lobby, readyScene, players as well for ease of reference
        // Start listening for ready and quit buttons
        base.OnStartLocalPlayer();
        displayName = GameObject.FindGameObjectWithTag("NetworkManager").
            GetComponent<NetworkLobbyManagerCustomised>().playerName;
        isAttack = GameObject.FindGameObjectWithTag("NetworkManager").
            GetComponent<NetworkLobbyManagerCustomised>().isAttack;
        CmdAddTeam(isAttack);
        CmdAddName(displayName, isAttack);
        CmdSendServerUpdateName();
        CmdSendServerUpdateSprite();
        CmdShowReadyStateUser(false, isAttack, teamIndex);
        lobby = GameObject.FindGameObjectWithTag("Lobby");
        readyScene = lobby.transform.GetChild(1).gameObject;
        message = readyScene.transform.GetChild(9).gameObject;

        players = Resources.LoadAll<GameObject>("SpawnablePrefabs/");
        // start coroutine is needed as the index of the newly joined client is not update immediately
        StartCoroutine(enableChooseCharacter());
        readyScene.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(ReadyUp);
        readyScene.transform.GetChild(10).GetComponent<Button>().onClick.AddListener(Quit);
    }

    private void Quit()
    {
        // Quit the game. Stopping host stops the game for all clients
        //if (index == 0)
        //{
        //    GameObject.FindGameObjectWithTag("NetworkManager").
        //        GetComponent<NetworkLobbyManagerCustomised>().StopHost(); //for now?
        //}
        //else
        //{
        //    if (isLocalPlayer)
        //    {
        //        // Start coroutine is needed as cmdremoveentry does not complete instantly at times
        //        CmdRemoveEntry(index, isAttack, teamIndex);
        //        StartCoroutine("removeClient");
        //    }
        //}
        if (isLocalPlayer)
        {
            // Start coroutine is needed as cmdremoveentry does not complete instantly at times
            CmdRemoveEntry(index, isAttack, teamIndex);
            StartCoroutine("removeClient");
        }
    }

    private void ReadyUp()
    {
        // When clients click on ready, they update the server and starts listening for unready event
        // For the host, clicking on ready does not ready yet for further confirmation with start game
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
            CmdShowReadyStateUser(true, isAttack, teamIndex);
        }
        else
        {
            // Clients must choose their character first before readying up
            message.GetComponent<TextMeshProUGUI>().SetText("Please Choose your character first!");
            // message will be removed after set time
            StartCoroutine(removeText());
        }
    }

    private void NotReady()
    {
        // updates server that client is not ready
        readyToBegin = false;
        CmdChangeReadyState(readyToBegin);
        CmdShowReadyStateUser(false, isAttack, teamIndex);
        lobby.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        lobby.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
    }

    private void StartGame()
    {
        // Starts game only when all clients are readied
        readyToBegin = true;
        CmdChangeReadyState(readyToBegin);
        if (GameObject.FindGameObjectWithTag("NetworkManager").
            GetComponent<NetworkLobbyManagerCustomised>().allPlayersReady == false)
        {
            message.GetComponent<TextMeshProUGUI>().SetText("Not every player is ready!");
            StartCoroutine(removeText());
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
        CmdAddSprite(i, teamIndex, isAttack);
    }

    private IEnumerator enableChooseCharacter()
    {
        yield return new WaitForSeconds(0.1f);
        Button chooseChar;
        if (isAttack)
        {
            if (readyScene.transform.GetChild(teamIndex + 3).GetChild(1).gameObject.GetComponent<Button>() == null)
            {
                chooseChar = readyScene.transform.GetChild(teamIndex + 3).GetChild(1).gameObject.AddComponent<Button>();

            }
            else
            {
                chooseChar = readyScene.transform.GetChild(teamIndex + 3).GetChild(1).gameObject.GetComponent<Button>();
                readyScene.transform.GetChild(teamIndex + 3).GetChild(1).gameObject.GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("shadedDark48");
            }
            chooseChar.onClick.AddListener(ChooseCharacter);
            readyScene.transform.GetChild(teamIndex + 3).GetChild(1).GetComponent<Image>().color =
                new Color(255f, 255f, 255f, 255f);
        }
        else
        {
            if (readyScene.transform.GetChild(teamIndex + 6).GetChild(1).gameObject.GetComponent<Button>() == null)
            {
                chooseChar = readyScene.transform.GetChild(teamIndex + 6).GetChild(1).gameObject.AddComponent<Button>();

            }
            else
            {
                chooseChar = readyScene.transform.GetChild(teamIndex + 6).GetChild(1).gameObject.GetComponent<Button>();
                readyScene.transform.GetChild(teamIndex + 6).GetChild(1).gameObject.GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("shadedDark48");
            }
            chooseChar.onClick.AddListener(ChooseCharacter);
            readyScene.transform.GetChild(teamIndex + 6).GetChild(1).GetComponent<Image>().color =
                new Color(255f, 255f, 255f, 255f);
        }
        
    }

    private IEnumerator removeClient()
    {
        yield return new WaitForSeconds(0.1f);
        if (isLocalPlayer)
        {
            GameObject.FindGameObjectWithTag("NetworkManager").
                GetComponent<NetworkLobbyManagerCustomised>().StopClient();
        }
    }

    private IEnumerator removeText()
    {
        yield return new WaitForSeconds(5f);
        if ((isLocalPlayer) && (SceneManager.GetActiveScene().name == "Lobby"))
        {
            message.GetComponent<TextMeshProUGUI>().SetText("");
        }
    }

    [Command]
    private void CmdAddTeam(bool isAttack)
    {
        LobbyResources.playerTeamAttack.Add(isAttack);
    }

    [Command]
    public void CmdShowReadyStateUser(bool isReady, bool isAtk, int teamIndex)
    {
        if (isAtk)
        {
            if (isReady) { LobbyResources.playerReadyStateAtk[teamIndex] = true; }
            else { LobbyResources.playerReadyStateAtk[teamIndex] = false; }
        }
        else
        {
            if (isReady) { LobbyResources.playerReadyStateDef[teamIndex] = true; }
            else { LobbyResources.playerReadyStateDef[teamIndex] = false; }
        }
        
        updateReadyStateClientRpc(LobbyResources.playerReadyStateAtk, LobbyResources.playerReadyStateDef);
    }

    [Command]
    public void CmdShowReadyStateUserNoChange()
    {
        updateReadyStateClientRpc(LobbyResources.playerReadyStateAtk, LobbyResources.playerReadyStateDef);
    }

    [Command]
    public void CmdSendServerUpdateName()
    {
        updateNamesClientRpc(LobbyResources.playerNamesAtk, LobbyResources.playerNamesDef);
    }

    [Command]
    public void CmdSendServerUpdateSprite()
    {
        updateSpritesClientRpc(LobbyResources.playerSpritesAtk, LobbyResources.playerIndexforSpriteAtk, LobbyResources.playerNamesAtk,
            LobbyResources.playerSpritesDef, LobbyResources.playerIndexforSpriteDef, LobbyResources.playerNamesDef);
    }

    [Command]
    public void CmdAddName(string name, bool isAtk)
    {
        if (isServer)
        {
            if (isAtk)
            {
                teamIndex = LobbyResources.playerNamesAtk.Count;
                LobbyResources.playerNamesAtk.Add(name);
                LobbyResources.playerReadyStateAtk.Add(false);
            }
            else
            {
                teamIndex = LobbyResources.playerNamesDef.Count;
                LobbyResources.playerNamesDef.Add(name);
                LobbyResources.playerReadyStateDef.Add(false);
            }
            
        }
    }

    [Command]
    public void CmdAddSprite(int sprite, int teamIndex, bool isAtk)
    {
        spriteIndex = sprite;
        if (isServer)
        {
            bool isPresent = false;
            if (isAtk)
            {
                for (int i = 0; i < LobbyResources.playerSpritesAtk.Count; i++)
                {
                    if (LobbyResources.playerIndexforSpriteAtk[i] == teamIndex)
                    {
                        LobbyResources.playerSpritesAtk[i] = sprite;
                        isPresent = false;
                    }
                }
                if (!isPresent)
                {
                    LobbyResources.playerSpritesAtk.Add(sprite);
                    LobbyResources.playerIndexforSpriteAtk.Add(teamIndex);
                }
            }
            else
            {
                for (int i = 0; i < LobbyResources.playerSpritesDef.Count; i++)
                {
                    if (LobbyResources.playerIndexforSpriteDef[i] == teamIndex)
                    {
                        LobbyResources.playerSpritesDef[i] = sprite;
                        isPresent = false;
                    }
                }
                if (!isPresent)
                {
                    LobbyResources.playerSpritesDef.Add(sprite);
                    LobbyResources.playerIndexforSpriteDef.Add(teamIndex);
                }
            }
            
        }
    }

    [Command]
    void CmdRemoveEntry(int index, bool isAtk, int teamIndex)
    {
        Debug.Log("Removing Entry");
        LobbyResources.playerTeamAttack.RemoveAt(index);
        if (isAtk)
        {
            LobbyResources.playerNamesAtk.RemoveAt(teamIndex);
            LobbyResources.playerReadyStateAtk.RemoveAt(teamIndex);
            int removeIndex = -1;
            for (int i = 0; i < LobbyResources.playerIndexforSpriteAtk.Count; i++)
            {
                if (LobbyResources.playerIndexforSpriteAtk[i] == teamIndex)
                {
                    removeIndex = i;
                }
                else if (LobbyResources.playerIndexforSpriteAtk[i] > teamIndex)
                {
                    LobbyResources.playerIndexforSpriteAtk[i]--;
                }
            }
            if (removeIndex != -1)
            {
                LobbyResources.playerIndexforSpriteAtk.RemoveAt(removeIndex);
                LobbyResources.playerSpritesAtk.RemoveAt(removeIndex);
                updateSpritesClientRpc(LobbyResources.playerSpritesAtk, LobbyResources.playerIndexforSpriteAtk, 
                    LobbyResources.playerNamesAtk, LobbyResources.playerSpritesDef, LobbyResources.playerIndexforSpriteDef,
                    LobbyResources.playerNamesDef);
            }
            updateNamesClientRpc(LobbyResources.playerNamesAtk, LobbyResources.playerNamesDef);
            updateReadyStateClientRpc(LobbyResources.playerReadyStateAtk, LobbyResources.playerReadyStateDef);
        }
        else
        {
            LobbyResources.playerNamesDef.RemoveAt(teamIndex);
            LobbyResources.playerReadyStateDef.RemoveAt(teamIndex);
            int removeIndex = -1;
            for (int i = 0; i < LobbyResources.playerIndexforSpriteDef.Count; i++)
            {
                if (LobbyResources.playerIndexforSpriteDef[i] == teamIndex)
                {
                    removeIndex = i;
                }
                else if (LobbyResources.playerIndexforSpriteDef[i] > teamIndex)
                {
                    LobbyResources.playerIndexforSpriteDef[i]--;
                }
            }
            if (removeIndex != -1)
            {
                LobbyResources.playerIndexforSpriteDef.RemoveAt(removeIndex);
                LobbyResources.playerSpritesDef.RemoveAt(removeIndex);
                updateSpritesClientRpc(LobbyResources.playerSpritesAtk, LobbyResources.playerIndexforSpriteAtk,
                    LobbyResources.playerNamesAtk, LobbyResources.playerSpritesDef, LobbyResources.playerIndexforSpriteDef,
                    LobbyResources.playerNamesDef);
            }
            updateNamesClientRpc(LobbyResources.playerNamesAtk, LobbyResources.playerNamesDef);
            updateReadyStateClientRpc(LobbyResources.playerReadyStateAtk, LobbyResources.playerReadyStateDef);
        }
        
    }

    [ClientRpc]
    public void updateReadyStateClientRpc(List<bool> playerReadyStateAtk, List<bool>playerReadyStateDef)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            readyScene = GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(1).gameObject;
            for (int i = 0; i < playerReadyStateAtk.Count; i++)
            {
                if (playerReadyStateAtk[i])
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
            for (int i = playerReadyStateAtk.Count; i < 3; i++)
            {
                readyScene.transform.GetChild(i + 3).GetChild(0).
                        GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
            }


            for (int i = 0; i < playerReadyStateDef.Count; i++)
            {
                if (playerReadyStateDef[i])
                {
                    readyScene.transform.GetChild(i + 6).GetChild(0).
                        GetComponent<TextMeshProUGUI>().color = new Color(57 / 255.0f, 1f, 20 / 255.0f);
                }
                else
                {
                    readyScene.transform.GetChild(i + 6).GetChild(0).
                        GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                }
            }
            for (int i = playerReadyStateDef.Count; i < 3; i++)
            {
                readyScene.transform.GetChild(i + 6).GetChild(0).
                        GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
            }
        }
    }

    [ClientRpc]
    public void updateSpritesClientRpc(List<int> playerSpritesAtk, List<int> playerIndexAtk, List<string> playerNamesAtk,
        List<int> playerSpritesDef, List<int> playerIndexDef, List<string> playerNamesDef)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            readyScene = GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(1).gameObject;
            players = Resources.LoadAll<GameObject>("SpawnablePrefabs/");

            for (int i = 0; i < playerSpritesAtk.Count; i++)
            {
                readyScene.transform.GetChild(playerIndexAtk[i] + 3).GetChild(1).GetComponent<RectTransform>().sizeDelta =
                    new Vector2(55, 70);
                readyScene.transform.GetChild(playerIndexAtk[i] + 3).GetChild(1).GetComponent<Image>().sprite =
                    players[playerSpritesAtk[i]].GetComponent<SpriteRenderer>().sprite;
                readyScene.transform.GetChild(playerIndexAtk[i] + 3).GetChild(1).GetComponent<Image>().color =
                    new Color(255f, 255f, 255f, 255f);
            }
            for (int i = playerSpritesAtk.Count; i < 3; i++)
            {
                if (i >= playerNamesAtk.Count)
                {
                    readyScene.transform.GetChild(i + 3).GetChild(1).GetComponent<RectTransform>().sizeDelta =
                    new Vector2(40, 40);
                    readyScene.transform.GetChild(i + 3).GetChild(1).GetComponent<Image>().color =
                        new Color(255f, 255f, 255f, 0f);
                }
            }

            for (int i = 0; i < playerSpritesDef.Count; i++)
            {
                readyScene.transform.GetChild(playerIndexDef[i] + 6).GetChild(1).GetComponent<RectTransform>().sizeDelta =
                    new Vector2(55, 70);
                readyScene.transform.GetChild(playerIndexDef[i] + 6).GetChild(1).GetComponent<Image>().sprite =
                    players[playerSpritesDef[i]].GetComponent<SpriteRenderer>().sprite;
                readyScene.transform.GetChild(playerIndexDef[i] + 6).GetChild(1).GetComponent<Image>().color =
                    new Color(255f, 255f, 255f, 255f);
            }
            for (int i = playerSpritesDef.Count; i < 3; i++)
            {
                if (i >= playerNamesDef.Count)
                {
                    readyScene.transform.GetChild(i + 6).GetChild(1).GetComponent<RectTransform>().sizeDelta =
                    new Vector2(40, 40);
                    readyScene.transform.GetChild(i + 6).GetChild(1).GetComponent<Image>().color =
                        new Color(255f, 255f, 255f, 0f);
                }
            }
        }
    }

    [ClientRpc]
    public void updateNamesClientRpc(List<string> playerNamesAtk, List<string> playerNamesDef)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            GameObject readyScene = GameObject.FindGameObjectWithTag("Lobby").transform.GetChild(1).gameObject;

            for (int i = 0; i < playerNamesAtk.Count; i++)
            {
                readyScene.transform.GetChild(i + 3).GetChild(0).GetComponent<TextMeshProUGUI>().
                    SetText(playerNamesAtk[i]);
            }
            for (int i = playerNamesAtk.Count; i < 3; i++)
            {
                readyScene.transform.GetChild(i + 3).GetChild(0).GetComponent<TextMeshProUGUI>().
                    SetText("");
            }

            for (int i = 0; i < playerNamesDef.Count; i++)
            {
                readyScene.transform.GetChild(i + 6).GetChild(0).GetComponent<TextMeshProUGUI>().
                    SetText(playerNamesDef[i]);
            }
            for (int i = playerNamesDef.Count; i < 3; i++)
            {
                readyScene.transform.GetChild(i + 6).GetChild(0).GetComponent<TextMeshProUGUI>().
                    SetText("");
            }
        }
    }
    
    
    
}
