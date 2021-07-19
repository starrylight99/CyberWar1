using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.EventSystems;

public class StartComputer : NetworkBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    float r = 10.0f;
    private void OnMouseDown()
    {
        player = NetworkClient.localPlayer.gameObject;
        Vector3 player_pos = player.transform.position;
        Vector3 curr_pos = transform.position;
        States playerStates = player.GetComponent<States>();
        if (!playerStates.saboCD || !playerStates.intelCD)
        {
            if ((Mathf.Abs(curr_pos.x - player_pos.x) <= r) && (Mathf.Abs(curr_pos.y - player_pos.y) <= r))
            {
                GameObject.FindGameObjectWithTag("RoomEventSystem").GetComponent<EventSystem>().enabled = false;
                player.transform.GetChild(0).GetComponent<AudioListener>().enabled = false;
                player.GetComponent<Rigidbody2D>().constraints =
                    RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                player.GetComponent<Rigidbody2D>().freezeRotation = true;
                SceneManager.LoadScene("ChooseComputerGame", LoadSceneMode.Additive);
                player.GetComponent<States>().playingMinigame = true;
            }
        }
        
    }
}
