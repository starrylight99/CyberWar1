using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class States : NetworkBehaviour
{
    [SerializeField]
    public bool winIntelGame = false;
    [SerializeField]
    public bool winSaboGame = false;
    [SerializeField]
    public int resourceInventory = 0;
    // Start is called before the first frame update
    void Start()
    {
        
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
}
