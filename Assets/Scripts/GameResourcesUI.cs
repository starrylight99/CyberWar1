using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
public class GameResourcesUI : NetworkBehaviour {
    private States player;

    [SerializeField] 
    TextMeshProUGUI resourceTMP;
    [SerializeField]
    TextMeshProUGUI resourceFOWTMP;
    static int teamResources;
    GameObject introPart1;
    GameObject introPart2;
    
    private void Start() {
        //GameObject.FindGameObjectWithTag("Button").GetComponent<Button>().onClick.AddListener(back);
        GameResources.OnResourceAmountChanged += delegate (object sender, EventArgs e) {
            /* player = NetworkClient.localPlayer.gameObject.GetComponent<States>();
            teamResources = GameResources.GetGoldAmount(player.isAttack); */
            //Debug.Log("GameResourcesUI: " + GameResources.GetGoldAmount(player.isAttack));
            //player.SetResourcesServer(teamResources, player.isAttack);
            //erverCommandUpdateResource();
            ServerCommandUpdateResource();
        };
        GameResources.OnFOWAmountChanged += delegate (object sender, EventArgs e) {
            ServerCommandUpdateFOWResource();
        };
        teamResources = 0;
        resourceTMP.SetText("Energy: 0");
        resourceFOWTMP.SetText("Stardust: 0");
        introPart1 = transform.GetChild(5).gameObject;
        introPart2 = transform.GetChild(6).gameObject;
        introPart1.SetActive(true);
        introPart2.SetActive(false);
        introPart1.GetComponentInChildren<Button>().onClick.AddListener(NextPage);
    }

    private void NextPage()
    {
        introPart1.SetActive(false);
        introPart2.SetActive(true);
        introPart2.GetComponentInChildren<Button>().onClick.AddListener(ClosePage);
    }

    private void ClosePage()
    {
        introPart1.SetActive(false);
        introPart2.SetActive(false);
    }

    [Server]
    private void ServerCommandUpdateResource(){
        UpdateResource(GameResources.atkResourceAmount, GameResources.defResourceAmount);
    }

    [Server]
    private void ServerCommandUpdateFOWResource()
    {
        UpdateFOWResource(GameResources.atkFOWAmt, GameResources.defFOWAmt);
    }

    [ClientRpc]
    public void UpdateResource(int atk, int def) {
        GameResources.atkResourceAmount = atk;
        GameResources.defResourceAmount = def;
        player = NetworkClient.localPlayer.gameObject.GetComponent<States>();
        teamResources = GameResources.GetGoldAmount(player.isAttack);
        resourceTMP.SetText("Energy: " + teamResources);
    }

    [ClientRpc]
    public void UpdateFOWResource(int atk, int def)
    {
        GameResources.atkFOWAmt = atk;
        GameResources.defFOWAmt = def;
        player = NetworkClient.localPlayer.gameObject.GetComponent<States>();
        teamResources = GameResources.GetFOWAmount(player.isAttack);
        resourceFOWTMP.SetText("Stardust: " + teamResources);
    }
}