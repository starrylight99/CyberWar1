using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartGenerator : NetworkBehaviour
{
    GameObject player;
    float r = 8f;
    Camera miningCam,playerCam;
    RigidbodyConstraints2D originalConstraints;
    [SerializeField] GameObject backButton;
    GameObject[] miningCams;

    void Start()
    {
        backButton.GetComponent<Button>().onClick.AddListener(delegate {
            miningCam.enabled = false;
            playerCam.enabled = true;
            backButton.SetActive(false);
            player.GetComponent<Rigidbody2D>().constraints = originalConstraints;
            gameObject.GetComponent<StartGenerator>().enabled = true;
        });
        backButton.SetActive(false);
        miningCams = GameObject.FindGameObjectsWithTag("MiningCamera");
        foreach (GameObject cam in miningCams)
        {
            cam.GetComponent<Camera>().enabled = false;
        }
    }
    void OnMouseDown()
    {
        player = NetworkClient.localPlayer.gameObject;
        Vector3 player_pos = player.transform.position;
        Vector3 curr_pos = transform.position;
        if (!player.GetComponent<States>().generatorCD)
        {
            if ((Mathf.Abs(curr_pos.x - player_pos.x) <= r) && (Mathf.Abs(curr_pos.y - player_pos.y) <= r))
            {
                if (player.GetComponent<States>().sabotaged)
                {
                    GameObject.FindGameObjectWithTag("RoomEventSystem").GetComponent<EventSystem>().enabled = false;
                    player.GetComponent<States>().saboStatus = 1;
                    player.transform.GetChild(0).GetComponent<AudioListener>().enabled = false;
                    player.GetComponent<Rigidbody2D>().constraints =
                        RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                    player.GetComponent<Rigidbody2D>().freezeRotation = true;
                    SceneManager.LoadScene("SlidingPuzzleScene", LoadSceneMode.Additive);
                    player.GetComponent<States>().playingMinigame = true;
                }
                else
                {
                    player.transform.GetChild(0).GetComponent<AudioListener>().enabled = false;
                    originalConstraints = player.GetComponent<Rigidbody2D>().constraints;
                    player.GetComponent<Rigidbody2D>().constraints =
                        RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                    player.GetComponent<Rigidbody2D>().freezeRotation = true;

                    bool isAtk = player.GetComponent<States>().isAttack;

                    playerCam = player.transform.Find("Local Camera").GetComponent<Camera>();
                    miningCams = GameObject.FindGameObjectsWithTag("MiningCamera");
                    foreach (GameObject cam in miningCams)
                    {
                        if (cam.GetComponent<TeamTag>().isAttack == isAtk)
                        {
                            miningCam = cam.GetComponent<Camera>();
                        }
                        else
                        {
                            cam.GetComponent<Camera>().enabled = false;
                        }
                    }
                    playerCam.enabled = false;
                    miningCam.enabled = true;
                    backButton.SetActive(true);
                    gameObject.GetComponent<StartGenerator>().enabled = false;
                }
            }
        }
    }
}