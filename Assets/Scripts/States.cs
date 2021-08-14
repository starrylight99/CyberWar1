using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class States : NetworkBehaviour
{
    [SerializeField]
    public bool CompleteFirewallGame = false;
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
    [SyncVar]
    public bool sabotaged;
    [SyncVar]
    public bool isFixing;
    public int startTime;
    List<GameObject> atkPlayers,defPlayers;
    bool timeIsRunning;
    public bool playingMinigame = false;
    [SyncVar]
    public Vector3 spawnPos;
    RoomResources roomResources;
    GameHandler gameHandlerComponent;
    MiningAI miningAI;
    [SyncVar]
    public int teamIndex;
    public int resourcesGained = 0;
    private bool colorChanged = false;
    TextMeshProUGUI message;
    TextMeshProUGUI timer;
    public int timeleft = 120;

    public bool saboCD;
    public bool intelCD;
    public bool firewallCD;
    public bool generatorCD;

    private int _finishGame = 0;
    bool finalBattleUpdate = false;
    public int finishGame
    {
        get { return _finishGame; }
        set
        {
            _finishGame = value;
            if (_finishGame == 1) //FinishSaboGame
            {
                saboCD = true;
                StartCoroutine(GameCooldown(1, 30));
                finishGame = 0;
            }
            else if (_finishGame == 2) //FinishIntelGame
            {
                intelCD = true;
                StartCoroutine(GameCooldown(2, 10));
                finishGame = 0;
            }
            else if (_finishGame == 3) //FinishFirewallGame
            {
                firewallCD = true;
                StartCoroutine(GameCooldown(3, 30));
                finishGame = 0;
                if (isAttack)
                {
                    GameObject.FindGameObjectWithTag("Attack").transform.GetChild(2)
                        .GetComponent<SpriteRenderer>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
                }
                else
                {
                    GameObject.FindGameObjectWithTag("Defend").transform.GetChild(2)
                        .GetComponent<SpriteRenderer>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
                }
                GameObject.FindGameObjectWithTag("UI").transform.GetChild(8).GetChild(1).
                    GetChild(0).GetComponent<Image>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
            }
            else if (_finishGame == 4)
            {
                saboCD = true;
                intelCD = true;
                firewallCD = true;
                generatorCD = true;
                if (isAttack)
                {
                    GameObject.FindGameObjectWithTag("Attack").transform.GetChild(2)
                        .GetComponent<SpriteRenderer>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
                    GameObject.FindGameObjectWithTag("Attack").transform.GetChild(3)
                        .GetComponent<SpriteRenderer>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
                }
                else
                {
                    GameObject.FindGameObjectWithTag("Defend").transform.GetChild(2)
                        .GetComponent<SpriteRenderer>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
                    GameObject.FindGameObjectWithTag("Defend").transform.GetChild(3)
                        .GetComponent<SpriteRenderer>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
                }
                GameObject.FindGameObjectWithTag("UI").transform.GetChild(8).GetChild(0).
                    GetChild(0).GetComponent<Image>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
                GameObject.FindGameObjectWithTag("UI").transform.GetChild(8).GetChild(1).
                    GetChild(0).GetComponent<Image>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
                GameObject.FindGameObjectWithTag("UI").transform.GetChild(8).GetChild(2).
                    GetChild(0).GetComponent<Image>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
            }

            if (saboCD && intelCD)
            {
                if (isAttack)
                {
                    GameObject.FindGameObjectWithTag("Attack").transform.GetChild(1)
                        .GetComponent<SpriteRenderer>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
                }
                else
                {
                    GameObject.FindGameObjectWithTag("Defend").transform.GetChild(1)
                        .GetComponent<SpriteRenderer>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
                }
                GameObject.FindGameObjectWithTag("UI").transform.GetChild(8).GetChild(0).
                    GetChild(0).GetComponent<Image>().color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
            }
        }
    }

    private int _saboStatus;
    public int saboStatus
    {
        get { return _saboStatus; }
        set
        {
            _saboStatus = value;
            if (_saboStatus == 1) // player fixing sabotage
            {
                CmdFixSabotage(isAttack, true);
            }
            else if (_saboStatus == 2) // player fixed sabotage
            {
                CmdFixedSabotage(isAttack);
            }
            else if (_saboStatus == 3) // player cant fix sabotage
            {
                CmdFixSabotage(isAttack, false);
            }
        }
    }

    IEnumerator GameCooldown(int gameCD, int seconds)
    {
        yield return new WaitForSeconds((float)seconds);
        if (!SceneManager.GetActiveScene().name.Contains("FinalBattle"))
        {
            switch (gameCD)
            {
                case 1:
                    saboCD = false;
                    if (!saboCD || !intelCD)
                    {
                        if (isAttack)
                        {
                            GameObject.FindGameObjectWithTag("Attack").transform.GetChild(1)
                                .GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
                        }
                        else
                        {
                            GameObject.FindGameObjectWithTag("Defend").transform.GetChild(1)
                                .GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
                        }
                        GameObject.FindGameObjectWithTag("UI").transform.GetChild(8).GetChild(0).
                            GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                    }
                    break;
                case 2:
                    intelCD = false;
                    if (!saboCD || !intelCD)
                    {
                        if (isAttack)
                        {
                            GameObject.FindGameObjectWithTag("Attack").transform.GetChild(1)
                                .GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
                        }
                        else
                        {
                            GameObject.FindGameObjectWithTag("Defend").transform.GetChild(1)
                                .GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
                        }
                        GameObject.FindGameObjectWithTag("UI").transform.GetChild(8).GetChild(0).
                            GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                    }
                    break;
                case 3:
                    firewallCD = false;
                    if (isAttack)
                    {
                        GameObject.FindGameObjectWithTag("Attack").transform.GetChild(2)
                            .GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
                    }
                    else
                    {
                        GameObject.FindGameObjectWithTag("Defend").transform.GetChild(2)
                            .GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
                    }
                    GameObject.FindGameObjectWithTag("UI").transform.GetChild(8).GetChild(1).
                        GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                    break;
                default:
                    break;
            }
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        //NetworkClient.Ready();
        StartCoroutine(AwaitReady());
    }
    IEnumerator AwaitReady(){
        while(!NetworkClient.ready){
            Debug.Log("Still not ready");
            yield return new WaitForSeconds(0.5f);
        }
        CmdSetName();
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
        message = GameObject.FindGameObjectWithTag("UI").transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        timer = GameObject.FindGameObjectWithTag("UI").transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        while (timeleft > 0)
        {
            int minutes = (int)(timeleft / 60);
            int seconds = (int)(timeleft - minutes * 60);
            string text = minutes.ToString() + ":" + seconds.ToString();
            timer.SetText(text);
            yield return new WaitForSeconds(1f);
            timeleft--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (timeIsRunning)
            {
                if (timeleft < 5)
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
                    finishGame = 4;
                }
            }
            if (winIntelGame)
            {
                // to add notification on winning the minigame
                string newMsg = "Correct! Gained 2 Stardust!\n";
                StartCoroutine(setText(newMsg));
                CmdAddFOWResources(2, isAttack);
                Debug.Log("Win Substitution Cipher Game");
                winIntelGame = false;
            }
            if (winSaboGame)
            {
                // to add notification on winning the minigame
                CmdSabotaged(isAttack);
                string newMsg = "Correct! Sending Sabotage to Opponents!\n";
                StartCoroutine(setText(newMsg));
                Debug.Log("Win Sabotage Game");
                winSaboGame = false;
            }
            if (CompleteFirewallGame)
            {
                //Add points to game resource
                string newMsg = "Gained " + resourcesGained.ToString() + " Energy!";
                StartCoroutine(setText(newMsg));
                CmdAddResources(resourcesGained, isAttack);
                CompleteFirewallGame = false;
                resourcesGained = 0;
            }
            if (SceneManager.GetActiveScene().name.Contains("RoomScene"))
            {
                if (sabotaged && !colorChanged)
                {
                    if (isAttack)
                    {
                        GameObject.FindGameObjectWithTag("Attack").transform.GetChild(3).
                            GetComponent<SpriteRenderer>().color = new Color(76 / 255f, 76 / 255f, 76 / 255f);
                    }
                    else
                    {
                        GameObject.FindGameObjectWithTag("Defend").transform.GetChild(3).
                            GetComponent<SpriteRenderer>().color = new Color(76 / 255f, 76 / 255f, 76 / 255f);
                    }
                    GameObject.FindGameObjectWithTag("UI").transform.GetChild(8).GetChild(2).
                        GetChild(0).GetComponent<Image>().color = new Color(76 / 255f, 76 / 255f, 76 / 255f);
                    colorChanged = true;
                    string newMsg = "Check your generator!\n";
                    StartCoroutine(setText(newMsg));
                }
                else if (!sabotaged && colorChanged && !isFixing)
                {
                    if (isAttack)
                    {
                        GameObject.FindGameObjectWithTag("Attack").transform.GetChild(3).
                            GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f);
                    }
                    else
                    {
                        GameObject.FindGameObjectWithTag("Defend").transform.GetChild(3).
                            GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f);
                    }
                    GameObject.FindGameObjectWithTag("UI").transform.GetChild(8).GetChild(2).
                        GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                    colorChanged = false;
                    string newMsg = "Generator Fixed!\n";
                    StartCoroutine(setText(newMsg));
                    saboStatus = 0;
                }
            }
            if (SceneManager.GetActiveScene().name == "FinalBattle") {
                atkPlayers = new List<GameObject>(); defPlayers = new List<GameObject>();
                GameObject[] playersArr = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in playersArr)
                {
                    if (player.GetComponent<States>().isAttack){
                        atkPlayers.Add(player);
                    }
                    else {
                        defPlayers.Add(player);
                    }
                }
                GameObject localPlayer = NetworkClient.localPlayer.gameObject;
                Light2D playerLight = NetworkClient.localPlayer.gameObject.transform.Find("Vision").gameObject.transform.Find("Close Light").gameObject.GetComponent<Light2D>();

                if (localPlayer.GetComponent<States>().isAttack) {
                    foreach (GameObject defPlayer in defPlayers)
                    {
                        Behaviour name = (Behaviour)(defPlayer.GetComponentInChildren(typeof(TextMeshProUGUI), true));
                        foreach (GameObject atkPlayer in atkPlayers)
                        {
                            if (Vector3.Distance(atkPlayer.transform.position, defPlayer.transform.position) < playerLight.pointLightOuterRadius){
                                goto EnableA;
                            }
                        }
                        goto DisableA;

                        EnableA:
                            name.enabled = true;
                            break;
                        DisableA:
                            name.enabled = false;
                            break;
                    }
                } else {
                    foreach (GameObject atkPlayer in atkPlayers)
                    {
                        Behaviour name = (Behaviour)(atkPlayer.GetComponentInChildren(typeof(TextMeshProUGUI), true));
                        foreach (GameObject defPlayer in defPlayers)
                        {
                            if (Vector3.Distance(atkPlayer.transform.position, defPlayer.transform.position) < playerLight.pointLightOuterRadius){
                                goto EnableB;
                            }
                        }
                        goto DisableB;

                        EnableB:
                            name.enabled = true;
                            break;
                        DisableB:
                            name.enabled = false;
                            break;
                    }
                }
            }
        } else {
            if (!finalBattleUpdate && SceneManager.GetActiveScene().name.Contains("FinalBattle")){
                finalBattleUpdate = true;
                GetComponentInChildren<TextMeshProUGUI>().SetText(displayName);

                if (NetworkClient.localPlayer.gameObject.GetComponent<States>().isAttack == isAttack){
                    GameObject mainVision = transform.Find("Vision").gameObject;

                    // Uncomment for variable vision
                    Light2D light = mainVision.transform.GetChild(0).GetComponent<Light2D>();
                    Light2D vision = mainVision.transform.GetChild(1).GetComponent<Light2D>();

                    light.pointLightOuterRadius = GameResources.GetFOWAmount(isAttack);
                    light.pointLightInnerRadius = GameResources.GetFOWAmount(isAttack)/4*3;
                    vision.pointLightOuterRadius = GameResources.GetFOWAmount(isAttack);
                    vision.pointLightInnerRadius = GameResources.GetFOWAmount(isAttack)/4*3;

                    mainVision.SetActive(true);
                }
                Debug.Log("Attack Gold:" +GameResources.GetGoldAmount(true));
                Debug.Log("Def Gold:" +GameResources.GetGoldAmount(false));
                Debug.Log("Attack fow:" +GameResources.GetFOWAmount(true));
                Debug.Log("Def fow:" +GameResources.GetFOWAmount(false));
                if (isAttack)
                {
                    GetComponentInChildren<TextMeshProUGUI>().color = new Color(255/255.0f, 185/255.0f, 185/255.0f);
                }
                else
                {
                    GetComponentInChildren<TextMeshProUGUI>().color = new Color(184/255.0f, 233/255.0f, 255/255.0f);
                }
            }
        }
    }
    IEnumerator setText(string newMsg)
    {
        message.SetText(message.text + newMsg);
        int seconds = 5;
        while (seconds > 0)
        {
            yield return new WaitForSeconds(1f);
            if (SceneManager.sceneCount > 1)
            {
                message.SetText("");
            }
            seconds--;
        }
        string newText = message.text.Replace(newMsg, "");
        message.SetText(newText);
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

    [Command]
    void CmdAddResources(int amount, bool isAttack)
    {
        GameResources.AddGoldAmount(amount, isAttack);
    }

    [Command]
    void CmdAddFOWResources(int amount, bool isAttack)
    {
        GameResources.AddFOWAmount(amount, isAttack);
    }

    [Command]
    void CmdSabotaged(bool isAttack)
    {
        GameObject oppMiner;
        if (isAttack)
        {
            oppMiner = GameObject.FindGameObjectWithTag("DefMiner");
        }
        else
        {
            oppMiner = GameObject.FindGameObjectWithTag("AtkMiner");
        }
        oppMiner.GetComponent<MiningAI>().sabotaged = true;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            States playerState = player.GetComponent<States>();
            if (playerState.isAttack != isAttack)
            {
                playerState.sabotaged = true;
            }
        }
    }

    [Command]
    void CmdFixSabotage(bool isAttack, bool fixing)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            States playerState = player.GetComponent<States>();
            if (playerState.isAttack == isAttack)
            {
                playerState.sabotaged = !fixing;
                if (fixing)
                {
                    playerState.isFixing = fixing;
                }
            }
        }
    }

    [Command]
    void CmdFixedSabotage(bool isAttack)
    {
        GameObject miner;
        if (isAttack)
        {
            miner = GameObject.FindGameObjectWithTag("AtkMiner");
        }
        else
        {
            miner = GameObject.FindGameObjectWithTag("DefMiner");
        }
        miner.GetComponent<MiningAI>().sabotaged = false;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            States playerState = player.GetComponent<States>();
            if (playerState.isAttack == isAttack)
            {
                playerState.isFixing = false;
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

}
