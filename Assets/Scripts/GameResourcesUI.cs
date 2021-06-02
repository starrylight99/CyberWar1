using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameResourcesUI : MonoBehaviour {
    public GameObject player;
    private bool updatable;
    private void Awake() {
        GameObject.FindGameObjectWithTag("GameHandler").transform.position = Vector3.zero;
        player = GameObject.FindGameObjectWithTag("Player");
        player.SetActive(false);
        updatable = true;
        GameObject.FindGameObjectWithTag("Button").GetComponent<Button>().onClick.AddListener(back);
        GameResources.OnResourceAmountChanged += delegate (object sender, EventArgs e) {
            UpdateResourceTextObject();
        };
        UpdateResourceTextObject();
    }

    private void back()
    {
        player.GetComponent<States>().resourceInventory = GameResources.GetGoldAmount();
        GameObject.FindGameObjectWithTag("GameHandler").transform.position = new Vector3(1000f, 1000f, 1000f);
        updatable = false;
        player.SetActive(true);
        SceneManager.LoadScene("RoomScene");

    }

    private void UpdateResourceTextObject() {
        Debug.Log("Here: "+ GameResources.GetGoldAmount().ToString());
        if (updatable)
            transform.GetChild(0).GetComponent<Text>().text = "Burgers: " + GameResources.GetGoldAmount();

    }

   
}