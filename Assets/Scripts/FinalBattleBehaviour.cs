using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FinalBattleBehaviour : NetworkBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool isAttack = GetComponent<States>().isAttack;
        if ((collision.gameObject.name.Equals("PlayerCollisionBlocker")) &&
            (isAttack != collision.transform.parent.GetComponent<States>().isAttack))
        {
            Debug.Log("Changing Spawn");
            if (isAttack)
            {
                transform.position = GetComponent<States>().spawnPos;
            }
            else
            {
                collision.transform.parent.position = collision.transform.parent.GetComponent<States>().spawnPos;
            }
        }
        else if (collision.gameObject.name.Equals("Flag"))
        {
            if (isAttack)
            {
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
                        roomPlayer.GetComponent<NetworkRoomPlayerScript>().winGame = 1;
                    }
                    else
                    {
                        roomPlayer.GetComponent<NetworkRoomPlayerScript>().winGame = -1;
                    }
                }
            }
        }
    }

    

}
