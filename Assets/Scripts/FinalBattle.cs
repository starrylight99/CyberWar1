using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class FinalBattle : MonoBehaviour
{   
    public Configuration configuration;
    public bool isAttack;
    float timer;
    public GameObject player;
    [SerializeField] TextMeshProUGUI showTimer;
    // Start is called before the first frame update
    void Start()
    {
        configuration = GameObject.FindGameObjectWithTag("Configuration").GetComponent<Configuration>();
        Debug.Log("FinalBattle Script Started");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.transform.position = player.GetComponent<States>().spawnPos;
        }
        timer = 120f;
        DisplayText(timer);
        StartCoroutine(SetText());
    }
    IEnumerator SetText()
    {
        if (configuration.buildType != BuildType.REMOTE_SERVER){
            yield return new WaitForSeconds(0.05f);
            GameObject InfoScreen = GameObject.FindGameObjectWithTag("UI").transform.GetChild(6).gameObject;
            InfoScreen.SetActive(true);
            player.GetComponent<Move>().canMove = false;
            if (isAttack)
            {
                InfoScreen.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                InfoScreen.transform.GetChild(2).gameObject.SetActive(true);
            }
            InfoScreen.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(Close);
        }
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

    void Close()
    {
        GameObject InfoScreen = GameObject.FindGameObjectWithTag("UI").transform.GetChild(6).gameObject;
        InfoScreen.SetActive(false);
        player.GetComponent<Move>().canMove = true;
    }


}
