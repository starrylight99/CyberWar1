using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class StartComputer : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    float r = 5.0f;
    void Awake()
    {
        // change to ClientScene.localPlayer.gameObject when integrating multiplayer
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseDown()
    {
        Debug.Log("Clicked");
        Vector3 player_pos = player.transform.position;
        Vector3 curr_pos = transform.position;
        if ((Mathf.Abs(curr_pos.x - player_pos.x) <= r) && (Mathf.Abs(curr_pos.y - player_pos.y) <= r))
        {
            Debug.Log("Inside");
            SceneManager.LoadScene("ChooseComputerGame");
        }
        else
        {
            Debug.Log("Please go in closer");
        }
    }
}
