using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using SQLite4Unity3d;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class getPhrase
{
    public string phrase { get; set; }
}

public class SubstQn : MonoBehaviour
{
    System.Random rng = new System.Random();
    GameObject inst;
    GameObject part1;
    GameObject part2;
    GameObject player;
    public int numHints;
    string key_arr = "";
    string shuffle = "abcdefghijklmnopqrstuvwxyz";
    string range = "abcdefghijklmnopqrstuvwxyz";
    string answer;

    void Start()
    {
        SceneManager.UnloadSceneAsync("ChooseComputerGame");
        inst = transform.GetChild(0).gameObject;
        part1 = transform.GetChild(1).gameObject;
        part2 = transform.GetChild(2).gameObject;
        foreach (GameObject players in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (players.GetComponent<States>().isLocalPlayer)
            {
                player = players;
            }
        }
        inst.SetActive(true);
        part1.SetActive(false);
        part2.SetActive(false);
        inst.GetComponentInChildren<Button>().onClick.AddListener(InstToPart1);
    }

    // Update is called once per frame
    void Update()
    {
        if (inst.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                InstToPart1();
            }
        }
        else if (part1.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Part1ToPart2();
            }
            
        }
        else if (part2.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                submit();
            }
        }
    }

    void InstToPart1()
    {
        inst.SetActive(false);
        part1.SetActive(true);
        shuffle = Shuffle(shuffle);
        string example = "the quick brown fox jumps over the lazy dog.";
        example = example + "\n" + encryptSubst(example, shuffle);
        part1.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(example);
        part1.GetComponentInChildren<Button>().onClick.AddListener(Part1ToPart2);
        giveHint(numHints);
    }

    void Part1ToPart2()
    {
        GameObject keyField = part1.transform.GetChild(0).GetChild(1).gameObject;
        for (int i = 0; i < 26; i++)
        {
            string inputted = "";
            GameObject obj = keyField.transform.GetChild(i).GetChild(0).gameObject;
            if (obj.activeInHierarchy)
            {
                inputted = obj.GetComponent<TMP_InputField>().text;
                if (inputted == "")
                {
                    inputted = " ";
                }
            }
            else
            {
                inputted = keyField.transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text;
            }
            key_arr += inputted;
        }
        part1.SetActive(false);
        part2.SetActive(true);
        startQn();
        part2.GetComponentInChildren<Button>().onClick.AddListener(submit);
        GameObject derivedKeyField = part2.transform.GetChild(0).GetChild(1).gameObject;
        for (int i = 0; i < 26; i++)
        {
            derivedKeyField.transform.GetChild(i).GetChild(0).
                GetComponent<TextMeshProUGUI>().SetText(key_arr[i].ToString());
        }
        
    }

    void submit()
    {
        string input = part2.transform.GetChild(2).GetComponent<TMP_InputField>().text;
        if (input == answer)
        {
            Debug.Log("correct");
            player.GetComponent<States>().winIntelGame = true;
        }
        else
        {
            Debug.Log("wrong");
        }
        player.transform.GetChild(0).gameObject.SetActive(true);
        player.transform.GetChild(0).GetComponent<AudioListener>().enabled = true;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.GetComponent<States>().playingMinigame = false;
        SceneManager.UnloadSceneAsync("IntelGameScene");
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

    void giveHint(int n)
    {
        range = Shuffle(range);
        for (int i = 0; i < n; i++)
        {
            int letter = range[i] - 'a';
            
            GameObject box = part1.transform.GetChild(0).GetChild(1).GetChild(letter).gameObject;
            box.transform.GetChild(0).gameObject.SetActive(false);
            GameObject text = new GameObject().gameObject;
            text.transform.SetParent(box.transform);
            configure(text, letter);
        }
    }

    void configure(GameObject text, int letter)
    {
        text.AddComponent<TextMeshProUGUI>().SetText(shuffle[letter].ToString());
        text.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
        text.GetComponent<TextMeshProUGUI>().fontSize = 10;
        text.GetComponent<TextMeshProUGUI>().color = new Color(0f, 0f, 0f);
        text.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
    }

    string encryptSubst(string input, string shuffle)
    {
        string cipher = "";
        for (int i = 0; i < input.Length; i++)
        {
            if ((input[i] == '.') || (input[i] == ' '))
            {
                cipher += input[i];
                continue;
            }
            int index = input[i] - 'a';
            cipher += shuffle[index];
        }
        return cipher;
    }

    void startQn()
    {
        int choice = rng.Next(1, 10);
        SQLiteConnection _db = new SQLiteConnection(Application.streamingAssetsPath + "/phrases.db");
        string query = $"SELECT * FROM phrases WHERE ROWID = " + choice.ToString();
        answer = _db.Query<getPhrase>(query)[0].phrase;
        Debug.Log(answer);
        _db.Close();
        string encrypted = encryptSubst(answer, shuffle);
        part2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().
            SetText("Decrypt this phrase: " + encrypted);
    }
}
