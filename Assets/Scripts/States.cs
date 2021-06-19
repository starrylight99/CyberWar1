using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

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

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetName();
    }


    // Update is called once per frame
    void Update()
    {
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
            player.GetComponentInChildren<TextMeshProUGUI>().SetText(player.GetComponent<States>().displayName);
        }
    }

}
