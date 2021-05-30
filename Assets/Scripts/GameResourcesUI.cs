using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameResourcesUI : MonoBehaviour {

    private void Awake() {
        GameResources.OnResourceAmountChanged += delegate (object sender, EventArgs e) {
            UpdateResourceTextObject();
        };
        UpdateResourceTextObject();
    }

    private void UpdateResourceTextObject() {
        transform.Find("Resources").GetComponent<Text>().text = "Burgers: " + GameResources.GetGoldAmount();
    }

}