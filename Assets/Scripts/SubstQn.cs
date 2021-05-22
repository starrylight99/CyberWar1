using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using SQLite4Unity3d;
using UnityEngine.SceneManagement;

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
    string key_arr = "";
    string shuffle = "abcdefghijklmnopqrstuvwxyz";
    string answer;
    float speed;

    void Start()
    {
        inst = transform.GetChild(0).gameObject;
        part1 = transform.GetChild(1).gameObject;
        part2 = transform.GetChild(2).gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        speed = player.GetComponent<Move>().speed;
        player.GetComponent<Move>().speed = 0f;
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
        
    }

    void Part1ToPart2()
    {
        GameObject keyField = part1.transform.GetChild(0).GetChild(1).gameObject;
        for (int i = 0; i < 26; i++)
        {
            string inputted = keyField.transform.GetChild(i).GetChild(0).
                GetComponent<TMP_InputField>().text;
            if (inputted == "")
            {
                inputted = " ";
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
            player.GetComponent<Move>().winGameA = true;
        }
        else
        {
            Debug.Log("wrong");
        }
        player.GetComponent<Move>().speed = speed;
        SceneManager.LoadScene("RoomScene");
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
        SQLiteConnection _db = new SQLiteConnection(Path.GetFullPath("Assets") + "/Scripts/phrases.db");
        string query = $"SELECT * FROM phrases WHERE ROWID = " + choice.ToString();
        answer = _db.Query<getPhrase>(query)[0].phrase;
        Debug.Log(answer);
        _db.Close();
        string encrypted = encryptSubst(answer, shuffle);
        part2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().
            SetText("Decrypt this phrase: " + encrypted);
    }
}
