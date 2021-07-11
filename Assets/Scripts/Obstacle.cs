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
