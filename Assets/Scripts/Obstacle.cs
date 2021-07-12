using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Obstacle : NetworkBehaviour
{
    [SyncVar] public bool isAttack;

    private void Start()
    {
        if (isServer)
        {
            StartCoroutine(trapSpawnTime());
        }
        if (isAttack)
        {
            GetComponent<SpriteRenderer>().color = new Color(255 / 255.0f, 185 / 255.0f, 185 / 255.0f);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(184 / 255.0f, 233 / 255.0f, 255 / 255.0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<States>().isAttack != isAttack)
            {
                collision.gameObject.GetComponent<Move>().slowed = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<States>().isAttack != isAttack)
            {
                collision.gameObject.GetComponent<Move>().slowed = false;
            }
        }
    }

    IEnumerator trapSpawnTime()
    {
        yield return new WaitForSeconds(10f);
        NetworkServer.Destroy(gameObject);
    }

}
