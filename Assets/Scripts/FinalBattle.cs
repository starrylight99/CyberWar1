using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class FinalBattle : MonoBehaviour
{
    float timer;
    [SerializeField] TextMeshProUGUI showTimer;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.transform.position = player.GetComponent<States>().spawnPos;
        }
        timer = 120f;
        DisplayText(timer);
    }

    private void DisplayText(float timer)
    {
        int minutes = (int)(timer / 60);
        int seconds = (int)(timer - minutes * 60);
        string text = minutes.ToString() + ":" + seconds.ToString();
        showTimer.SetText(text);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0f)
        {
            timer = 0f;
            GameObject[] roomPlayers = GameObject.FindGameObjectsWithTag("RoomPlayer");
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                player.GetComponent<Rigidbody2D>().constraints =
                    RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            }
            foreach (GameObject roomPlayer in roomPlayers)
            {
                if (roomPlayer.GetComponent<NetworkRoomPlayerScript>().isAttack)
                {
                    roomPlayer.GetComponent<NetworkRoomPlayerScript>().winGame = -1;
                }
                else
                {
                    roomPlayer.GetComponent<NetworkRoomPlayerScript>().winGame = 1;
                }
            }
        }
        timer -= Time.deltaTime;
        DisplayText(timer);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log(collision.gameObject);
    //    if ((collision.gameObject.tag.Equals("Player")) && (playable))
    //    {
    //        if (collision.gameObject.GetComponent<States>().isAttack)
    //        {
    //            GameObject[] roomPlayers = GameObject.FindGameObjectsWithTag("RoomPlayer");
    //            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    //            foreach (GameObject player in players)
    //            {
    //                player.GetComponent<Rigidbody2D>().constraints = 
    //                    RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
    //            }
    //            foreach (GameObject roomPlayer in roomPlayers)
    //            {
    //                if (roomPlayer.GetComponent<NetworkRoomPlayerScript>().isAttack)
    //                {
    //                    roomPlayer.GetComponent<NetworkRoomPlayerScript>().winGame = 1;
    //                }
    //                else
    //                {
    //                    roomPlayer.GetComponent<NetworkRoomPlayerScript>().winGame = -1;
    //                }
    //            }
    //        }
    //    }
    //}


}
