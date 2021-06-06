using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;



public class StartComputer : NetworkBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    float r = 6.0f;
    void Awake()
    {   
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseDown()
    {
        player = NetworkClient.localPlayer.gameObject;
        Debug.Log("Clicked");
        Debug.Log(player);
        Vector3 player_pos = player.transform.position;
        Vector3 curr_pos = transform.position;
        if ((Mathf.Abs(curr_pos.x - player_pos.x) <= r) && (Mathf.Abs(curr_pos.y - player_pos.y) <= r))
        {
            Debug.Log("Inside");
            player.transform.GetChild(0).GetComponent<AudioListener>().enabled = false;
            SceneManager.LoadScene("ChooseComputerGame", LoadSceneMode.Additive);
            
        }
        else
        {
            Debug.Log("Please go in closer");
        }
    }

    
}
