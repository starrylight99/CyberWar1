using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Message
{
    public bool isReceiving { get; set; }
    public string msgObj { get; set; }
    public string location { get; set; }

}

public class Rules
{
    public string obj { get; set; }
    public bool isReceiving { get; set; }
    public string location { get; set; }
}

public class FirewallGame : MonoBehaviour
{
    private string[] objects = { "apples", "bananas", "water", "oxygen tanks", "classified files", "weapons",
                                 "medication", "waste", "fuel", "spacesuits" };
    private string[] locations = { "moon", "control tower", "earth", "satellite", "mars", "ISS", "pluto",
                                   "venus", "NASA", "singapore" };
    System.Random rng = new System.Random();
    GameObject inst1;
    GameObject inst2;
    GameObject game;
    GameObject player;
    int points = 0;
    List<Rules> definedRules = new List<Rules>();
    GameObject msgBox;
    TextMeshProUGUI msg;
    Message message;
    TextMeshProUGUI ptsText;
    Button accept;
    Button reject;
    // Start is called before the first frame update
    void Start()
    {
        inst1 = transform.GetChild(0).gameObject;
        inst2 = transform.GetChild(1).gameObject;
        game = transform.GetChild(2).gameObject;
        foreach (GameObject players in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (players.GetComponent<States>().isLocalPlayer)
            {
                player = players;
            }
        }
        if (SceneManager.sceneCount > 2)
        {
            int scenes = SceneManager.sceneCount;
            for (int i = 1; i < scenes - 1; i++)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
            }
        }
        player.transform.GetChild(0).gameObject.SetActive(false);
        inst1.SetActive(true);
        inst2.SetActive(false);
        game.SetActive(false);
        msgBox = game.transform.GetChild(3).GetChild(0).gameObject;
        msg = msgBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        ptsText = game.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
        accept = game.transform.GetChild(1).GetComponent<Button>();
        reject = game.transform.GetChild(2).GetComponent<Button>();
        inst1.GetComponentInChildren<Button>().onClick.AddListener(Inst1ToInst2);
    }

    private void Inst1ToInst2()
    {
        inst1.SetActive(false);
        inst2.SetActive(true);
        inst2.GetComponentInChildren<Button>().onClick.AddListener(Inst2ToGame);
    }

    private void Inst2ToGame()
    {
        inst2.SetActive(false);
        game.SetActive(true);
        ptsText.SetText("Points: 0");
        StartCoroutine(StartTimer(45, game.transform.GetChild(4).GetComponent<TextMeshProUGUI>()));
        SetRules();
        message = SetMessage();
        accept.onClick.AddListener(()=>SelectedAction(true));
        reject.onClick.AddListener(() => SelectedAction(false));
    }

    private void SelectedAction(bool choice)
    {
        accept.interactable = false;
        reject.interactable = false;
        bool ans = GetAnswer();
        if (ans == choice)
        {
            points += 1;
            ptsText.SetText("Points: " + points.ToString());
        }
        msgBox.SetActive(false);
        message = SetMessage();
        StartCoroutine(NextQn(rng.Next(3, 7)));
    }

    private IEnumerator NextQn(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        msgBox.SetActive(true);
        accept.interactable = true;
        reject.interactable = true;
    }

    private bool GetAnswer()
    {
        foreach (Rules rule in definedRules)
        {
            if (rule.obj.Equals(message.msgObj) && rule.isReceiving == message.isReceiving &&
                rule.location.Equals(message.location))
            {
                return true;
            }
        }
        return false;
    }

    private Message SetMessage()
    {
        bool correct = UnityEngine.Random.value < 0.5;
        string obj;
        string location;
        bool isReceiving;
        if (correct)
        {
            int index = rng.Next(0, definedRules.Count);
            isReceiving = definedRules[index].isReceiving;
            obj = definedRules[index].obj;
            location = definedRules[index].location;
        }
        else
        {
            isReceiving = UnityEngine.Random.value < 0.5;
            obj = objects[rng.Next(0, objects.Length)];
            location = locations[rng.Next(0, locations.Length)];
        }
        string flow1 = isReceiving ? "Receive" : "Send";
        string flow2 = flow1 == "Receive" ? "from" : "to";
        msg.SetText(flow1 + " " + obj + " " + flow2 + " " + location + "?");
        return new Message { isReceiving = isReceiving, location = location, msgObj = obj };
    }

    private void SetRules()
    {
        TextMeshProUGUI rules = game.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        int count = 1;
        string locOrder = Shuffle("0123456789");
        string objOrder = Shuffle("0123456789");
        string rule = "";
        while (count <= 4)
        {
            string obj = objects[objOrder[count] - '0'];
            bool isReceiving = UnityEngine.Random.value < 0.5;
            string flow = isReceiving ? "to be received from" : "to be sent to";
            string location = locations[locOrder[count] - '0'];
            rule += count.ToString() + ". Allow " + obj + " " + flow + " " + location + ".\n";
            count++;
            definedRules.Add(new Rules { isReceiving = isReceiving, location = location, obj = obj });
        }
        rule += count.ToString() + ". Block everything else.";
        rules.SetText(rule);
    }

    private IEnumerator StartTimer(int seconds, TextMeshProUGUI timer)
    {
        while (seconds > 0)
        {
            timer.SetText("Time Left: " + seconds.ToString());
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        timer.SetText("Time Left: " + seconds.ToString());
        EndGame();
    }

    private void EndGame()
    {
        StopAllCoroutines();
        States playerStates = player.GetComponent<States>();
        playerStates.CompleteFirewallGame = true;
        playerStates.resourcesGained = points;
        player.transform.GetChild(0).gameObject.SetActive(true);
        player.transform.GetChild(0).GetComponent<AudioListener>().enabled = true;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerStates.playingMinigame = false;
        playerStates.finishGame = 3;
        GameObject.FindGameObjectWithTag("RoomEventSystem").GetComponent<EventSystem>().enabled = true;
        SceneManager.UnloadSceneAsync("FirewallGameScene");
    }

    string Shuffle(string str)
    {
        char[] array = str.ToCharArray();
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
        return new string(array);
    }

}
