using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KnockBackObstacle : NetworkBehaviour
{
    [SyncVar] public bool isAttack;
    [SyncVar] public Vector3 dir;
    private void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<States>().isAttack == isAttack)
            {
                Physics2D.IgnoreCollision(GetComponent<PolygonCollider2D>(), player.GetComponent<BoxCollider2D>());
                Physics2D.IgnoreCollision(GetComponent<PolygonCollider2D>(), 
                    player.transform.GetChild(1).GetComponent<BoxCollider2D>());
            }
        }
        GetComponent<Rigidbody2D>().AddForce(3000 * dir);
        if (isAttack)
        {
            GetComponent<SpriteRenderer>().color = new Color(255 / 255.0f, 185 / 255.0f, 185 / 255.0f);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(184 / 255.0f, 233 / 255.0f, 255 / 255.0f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isServer)
        {
            
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<Move>().scramble = true;
            }
            NetworkServer.Destroy(gameObject);
        }

    }

}
