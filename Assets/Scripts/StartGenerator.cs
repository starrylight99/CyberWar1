using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CodeMonkey.Utils;

public class StartGenerator : MonoBehaviour
{
    GameObject generatorPlayer;
    BoxCollider2D generator;
    Button_Sprite button;
    float r = 6.5f;
    void Awake()
    {
        generator = GetComponent<BoxCollider2D>();
        generatorPlayer = GameObject.FindGameObjectWithTag("Player");
        this.GetComponent<Button_Sprite>().ClickFunc = () => {
            Debug.Log("Clicked");
            Vector2 player_pos = generatorPlayer.transform.position;
            Vector2 curr_pos = transform.position;
            Debug.Log(Vector2.Distance(player_pos,curr_pos));
            if (Vector2.Distance(player_pos,curr_pos)<= r)
            {
                Debug.Log("Inside");
                SceneManager.LoadScene("MiningScene");
            }
            else
            {
                Debug.Log("Please go in closer");
            }
        };
    }
}