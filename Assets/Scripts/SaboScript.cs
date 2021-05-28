using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Query
{
    public string header { get; set; }
    public string description { get; set; }
    public string correct_query { get; set; }
    public string wrong_query1 { get; set; }
    public string wrong_query2 { get; set; }

    public char correct_ans { get; set; }
    public char wrong_ans1 { get; set; }
    public char wrong_ans2 { get; set; }

    public int correct_pos { get; set; }
    public int wrong_pos1 { get; set; }
    public int wrong_pos2 { get; set; }

    public int font_size { get; set; }
}

public class SaboScript : MonoBehaviour
{
    System.Random rng = new System.Random();
    GameObject inst1;
    GameObject inst2;
    GameObject game;
    GameObject player;
    float speed;
    string displayText = "";
    string answer = "";
    public int numGames;
    string gameSeq = "";
    public LinkedListNode<Query> curr;

    UnityAction InfoAction1;
    UnityAction InfoAction2;
    UnityAction InfoAction3;
    // Start is called before the first frame update
    void Start()
    {
        inst1 = transform.GetChild(0).gameObject;
        inst2 = transform.GetChild(1).gameObject;
        game = transform.GetChild(2).gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        speed = player.GetComponent<Move>().speed;
        player.GetComponent<Move>().speed = 0f;
        inst1.SetActive(true);
        inst2.SetActive(false);
        game.SetActive(false);
        inst1.GetComponentInChildren<Button>().onClick.AddListener(Inst1ToInst2);
        for (int i = 1; i <= numGames; i++)
        {
            answer += (char) (rng.Next(0, 9) + '0');
            gameSeq += (char)(i + '0');
        }
        gameSeq = Shuffle(gameSeq);
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
        GameObject keypad = game.transform.GetChild(0).gameObject; // keypad must be first child
        for (int i = 0; i < 10; i++)
        {
            //make sure display is the last child
            GameObject button = keypad.transform.GetChild(i).gameObject;
            button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    addDigit(button.GetComponentInChildren<TextMeshProUGUI>().text, keypad);
                });
        }
        keypad.transform.GetChild(10).GetComponent<Button>().onClick.AddListener(() => 
            {
                removeText(keypad);
            }); //10th child is clr
        keypad.transform.GetChild(11).GetComponent<Button>().onClick.AddListener(submit); //11th child is submit
        
        SQLiteConnection _db = new SQLiteConnection(Path.GetFullPath("Assets") + "/Scripts/phrases.db");
        LinkedList<Query> llQuery = new LinkedList<Query>();
        for (int i = 0; i < numGames; i++)
        {
            string header, ans_query, wrong_query, description;
            switch (gameSeq[i])
            {
                case '1':
                    description = "A strong password should be uncommon, typically long and should contain both uppercase and special characters.";
                    header = (i + 1).ToString() + ". (Password) Choose the weakest password for your victim:";
                    ans_query = "SELECT password FROM passwords WHERE strength = 0 ORDER BY RANDOM() LIMIT 1";
                    wrong_query = "SELECT password FROM passwords WHERE strength = 1 ORDER BY RANDOM() LIMIT 1";
                    llQuery.AddLast(setQuery(header, description, ans_query, wrong_query, "Your victim's password is: \n<b>", answer[i] - '0', 18, _db));
                    break;
                case '2':
                    description = "Using Public WiFi can be dangerous as people could be listening on your activities. On Home WiFi, it is also important to secure your network to deter any form of malicious activity.";
                    header = (i + 1).ToString() + ". (WiFi) Choose the most unsecured action for your victim:";
                    ans_query = "SELECT * FROM wifi WHERE strength = 0 ORDER BY RANDOM() LIMIT 1;";
                    wrong_query = "SELECT * FROM wifi WHERE strength = 1 ORDER BY RANDOM() LIMIT 1";
                    llQuery.AddLast(setQuery(header, description, ans_query, wrong_query, "Your victim will \n<b>", answer[i] - '0', 13, _db));
                    break;
                case '3':
                    description = "Phishing is now one of the most common cyber attacks, often targeted towards individuals. To protect yourself from phishing attacks, you must be wary of email and browser contents.";
                    header = (i + 1).ToString() + ". (Phishing) Choose the most unsecured action for your victim:";
                    ans_query = "SELECT * FROM phishing WHERE strength = 0 ORDER BY RANDOM() LIMIT 1;";
                    wrong_query = "SELECT * FROM phishing WHERE strength = 1 ORDER BY RANDOM() LIMIT 1";
                    llQuery.AddLast(setQuery(header, description, ans_query, wrong_query, "Your victim will \n<b>", answer[i] - '0', 13, _db));
                    break;
                case '4':
                    description = "Social Engineering involves the divulgence of your personal information for malicious purposes. Therefore, you must be cautious about revealing your personal information, even to your 'friends' online.";
                    header = (i + 1).ToString() + ". (Social) Choose the riskiest situation for your victim:";
                    ans_query = "SELECT * FROM social WHERE strength = 0 ORDER BY RANDOM() LIMIT 1;";
                    wrong_query = "SELECT * FROM social WHERE strength = 1 ORDER BY RANDOM() LIMIT 1";
                    llQuery.AddLast(setQuery(header, description, ans_query, wrong_query, "<b>", answer[i] - '0', 14, _db));
                    break;
            }
        }
        _db.Close();
        GameObject choice = game.transform.GetChild(1).gameObject;
        choice.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(prev);
        choice.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(next);
        curr = llQuery.First;
        Query curr_q = curr.Value;
        choice.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText(curr_q.header);
        GameObject info = choice.transform.GetChild(4).gameObject;
        InfoAction1 = () => { moreInfo(curr_q.correct_query, info, curr_q.correct_ans, curr_q.font_size); };
        InfoAction2 = () => { moreInfo(curr_q.wrong_query1, info, curr_q.wrong_ans1, curr_q.font_size); };
        InfoAction3 = () => { moreInfo(curr_q.wrong_query2, info, curr_q.wrong_ans2, curr_q.font_size); };
        choice.transform.GetChild(curr_q.correct_pos).GetComponent<Button>().onClick.AddListener(InfoAction1);
        choice.transform.GetChild(curr_q.wrong_pos1).GetComponent<Button>().onClick.AddListener(InfoAction2);
        choice.transform.GetChild(curr_q.wrong_pos2).GetComponent<Button>().onClick.AddListener(InfoAction3);
    }

    private Query setQuery(string header, string theme, string ans_query, string wrong_query, string text, int digit, int font_size, SQLiteConnection _db)
    {
        string correct = text + _db.ExecuteScalar<string>(ans_query) + "</b>";
        string wrong1 = text + _db.ExecuteScalar<string>(wrong_query) + "</b>";
        string wrong2;
        do
        {
            wrong2 = text + _db.ExecuteScalar<string>(wrong_query) + "</b>";
        } while (wrong2.Equals(wrong1));
        int correct_pos = rng.Next(0, 2);
        int wrong_ans;
        int buffer;
        do
        {
            wrong_ans = rng.Next(0, 9);
            buffer = rng.Next(0, 9);
        } while ((wrong_ans == digit) || (buffer == digit) || (buffer == wrong_ans));
        return new Query
        {
            header = header, description = theme, correct_query = correct, wrong_query1 = wrong1,
            wrong_query2 = wrong2, correct_ans = (char) (digit + '0') , wrong_ans1 = (char) (wrong_ans + '0'),
            wrong_ans2 = (char) (buffer + '0'), correct_pos = correct_pos,
            wrong_pos1 = (correct_pos + 2) % 3, wrong_pos2 = (correct_pos + 4) % 3, font_size = font_size
        };
    }

    void prev()
    {
        GameObject choice = game.transform.GetChild(1).gameObject;
        GameObject info = choice.transform.GetChild(4).gameObject;
        choice.transform.GetChild(curr.Value.correct_pos).GetComponent<Button>().onClick.RemoveListener(InfoAction1);
        choice.transform.GetChild(curr.Value.wrong_pos1).GetComponent<Button>().onClick.RemoveListener(InfoAction2);
        choice.transform.GetChild(curr.Value.wrong_pos2).GetComponent<Button>().onClick.RemoveListener(InfoAction3);
        curr = curr.Previous;
        Query curr_q = curr.Value;
        choice.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText(curr_q.header);
        InfoAction1 = () => { moreInfo(curr_q.correct_query, info, curr_q.correct_ans, curr_q.font_size); };
        InfoAction2 = () => { moreInfo(curr_q.wrong_query1, info, curr_q.wrong_ans1, curr_q.font_size); };
        InfoAction3 = () => { moreInfo(curr_q.wrong_query2, info, curr_q.wrong_ans2, curr_q.font_size); };
        choice.transform.GetChild(curr_q.correct_pos).GetComponent<Button>().onClick.AddListener(InfoAction1);
        choice.transform.GetChild(curr_q.wrong_pos1).GetComponent<Button>().onClick.AddListener(InfoAction2);
        choice.transform.GetChild(curr_q.wrong_pos2).GetComponent<Button>().onClick.AddListener(InfoAction3);
        if (curr.Previous == null)
        {
            choice.transform.GetChild(5).gameObject.SetActive(false);
        }
        choice.transform.GetChild(6).gameObject.SetActive(true);
    }

    void next()
    {
        GameObject choice = game.transform.GetChild(1).gameObject;
        GameObject info = choice.transform.GetChild(4).gameObject;
        choice.transform.GetChild(curr.Value.correct_pos).GetComponent<Button>().onClick.RemoveListener(InfoAction1);
        choice.transform.GetChild(curr.Value.wrong_pos1).GetComponent<Button>().onClick.RemoveListener(InfoAction2);
        choice.transform.GetChild(curr.Value.wrong_pos2).GetComponent<Button>().onClick.RemoveListener(InfoAction3);
        curr = curr.Next;
        Query curr_q = curr.Value;
        choice.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText(curr_q.header);
        InfoAction1 = () => { moreInfo(curr_q.correct_query, info, curr_q.correct_ans, curr_q.font_size); };
        InfoAction2 = () => { moreInfo(curr_q.wrong_query1, info, curr_q.wrong_ans1, curr_q.font_size); };
        InfoAction3 = () => { moreInfo(curr_q.wrong_query2, info, curr_q.wrong_ans2, curr_q.font_size); };
        choice.transform.GetChild(curr_q.correct_pos).GetComponent<Button>().onClick.AddListener(InfoAction1);
        choice.transform.GetChild(curr_q.wrong_pos1).GetComponent<Button>().onClick.AddListener(InfoAction2);
        choice.transform.GetChild(curr_q.wrong_pos2).GetComponent<Button>().onClick.AddListener(InfoAction3);
        if (curr.Next == null)
        {    
            choice.transform.GetChild(6).gameObject.SetActive(false);
        }
        choice.transform.GetChild(5).gameObject.SetActive(true);
    }

    void moreInfo(string text, GameObject info, char digit, int font_size)
    {
        info.SetActive(true);
        game.transform.GetChild(1).GetChild(5).gameObject.SetActive(false);
        game.transform.GetChild(1).GetChild(6).gameObject.SetActive(false);
        text = text + "\nIf you choose this option, please key in <b>" + digit + "</b> in the KeyPad.";
        info.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(closeInfo);
        //foreach (Button button in game.transform.GetChild(0).GetComponentsInChildren<Button>()){
        //    button.interactable = false;
        //} //Uncomment if dont want player to type while on the card
        info.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = font_size;
        info.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(text);

    }

    private void closeInfo()
    {
        game.transform.GetChild(1).GetChild(4).gameObject.SetActive(false);
        game.transform.GetChild(1).GetChild(5).gameObject.SetActive(true);
        game.transform.GetChild(1).GetChild(6).gameObject.SetActive(true);
        if (curr.Previous == null)
        {
            game.transform.GetChild(1).GetChild(5).gameObject.SetActive(false);
        }else if (curr.Next == null)
        {
            game.transform.GetChild(1).GetChild(6).gameObject.SetActive(false);
        }
    }

    private void submit()
    {
        if (displayText.Equals(answer))
        {
            Debug.Log("Correct");
            player.GetComponent<Move>().winSaboGame = true;
        }
        else
        {
            Debug.Log("Wrong");
        }
        player.GetComponent<Move>().speed = speed;
        SceneManager.LoadScene("RoomScene");
    }

    private void removeText(GameObject keypad)
    {
        if (displayText.Length > 0)
        {
            displayText = displayText.Remove(displayText.Length - 1);
        }
        GameObject display = keypad.transform.GetChild(keypad.transform.childCount - 1).gameObject;
        display.GetComponentInChildren<TextMeshProUGUI>().SetText(displayText);
    }

    void addDigit(string text, GameObject keypad)
    {
        if (displayText.Length < 7)
        {
            displayText += text;
        }
            GameObject display = keypad.transform.GetChild(keypad.transform.childCount - 1).gameObject;
            display.GetComponentInChildren<TextMeshProUGUI>().SetText(displayText);
    }

    // Update is called once per frame
    void Update()
    {
        if (inst1.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Inst1ToInst2();
            }
        }
        else if (inst2.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Inst2ToGame();
            }

        }
        else if (game.activeInHierarchy)
        {
            
        }
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
