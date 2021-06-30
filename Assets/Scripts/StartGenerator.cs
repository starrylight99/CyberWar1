using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;

public class StartGenerator : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    float r = 6.5f;
    Camera miningCam,playerCam;
    RigidbodyConstraints2D originalConstraints;
    [SerializeField] GameObject backButton;
    GameObject[] miningCams;

    void Start()
    {
        backButton.GetComponent<Button>().onClick.AddListener(delegate {
            /* player = NetworkClient.localPlayer.gameObject;
            playerCam = GameObject.FindGameObjectWithTag("Player").transform.Find("Local Camera").GetComponent<Camera>();
            miningCam = transform.Find("Mining/Environment/MiningCamera").GetComponent<Camera>(); */
            /* Debug.Log(miningCam);
            Debug.Log(playerCam); */
            miningCam.enabled = false;
            playerCam.enabled = true;
            backButton.SetActive(false);
            player.GetComponent<Rigidbody2D>().constraints = originalConstraints;
        });
        backButton.SetActive(false);
    }
    void OnMouseDown()
    {
        player = NetworkClient.localPlayer.gameObject;
        Debug.Log("Clicked");
        Vector3 player_pos = player.transform.position;
        Vector3 curr_pos = transform.position;
        if ((Mathf.Abs(curr_pos.x - player_pos.x) <= r) && (Mathf.Abs(curr_pos.y - player_pos.y) <= r))
        {
            Debug.Log("Inside");
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
                if (cam.GetComponent<TeamTag>().isAttack == isAtk){
                    miningCam = cam.GetComponent<Camera>();
                } else {
                    cam.GetComponent<Camera>().enabled = false;
                }
            }
            /* Debug.Log(playerCam);
            Debug.Log(miningCam); */
            playerCam.enabled = false;
            miningCam.enabled = true;
            backButton.SetActive(true);
        } else {
            Debug.Log("Please go in closer");
        }
    }
}