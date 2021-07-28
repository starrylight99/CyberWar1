using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitSlidingPuzzle : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject players in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (players.GetComponent<States>().isLocalPlayer)
            {
                player = players;
            }
        }
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(EndGame);
    }

    private void EndGame()
    {
        player.GetComponent<States>().saboStatus = 3;
        player.transform.GetChild(0).gameObject.SetActive(true);
        player.transform.GetChild(0).GetComponent<AudioListener>().enabled = true;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.GetComponent<States>().playingMinigame = false;
        SceneManager.UnloadSceneAsync("SlidingPuzzleScene");
    }

}
