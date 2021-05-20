using UnityEngine;
using TMPro;
using SQLite4Unity3d;
using System.IO;
using UnityEngine.UI;

public class getPhrase
{
    public string phrase { get; set; }
}

public class EncryptionQn : MonoBehaviour
{
    private System.Random _rnd = new System.Random();
    private SQLiteConnection _db;
    public GameObject button;
    public GameObject inputField;
    public GameObject timer;
    public GameObject hint;
    public GameObject score_text;

    int score = 0;
    float maxTime = 120f;
    int select;
    string answer;
    private int offset;

    // Start is called before the first frame update
    void Start()
    {
        startNewQn();
        button.GetComponent<Button>().onClick.AddListener(submit);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            submit();
        }
        maxTime -= Time.deltaTime;
        if (maxTime <= 0)
        {
            gameOver();
        }
        else
        {
            timer.GetComponent<TextMeshProUGUI>().SetText(((int)(maxTime)).ToString() + "s");
        }
    }

    void startNewQn()
    {
        offset = _rnd.Next(1, 25);
        select = _rnd.Next(1, 10);
        _rnd = new System.Random();
        showHint();
        _db = new SQLiteConnection(Path.GetFullPath("Assets") + "/Scripts/phrases.db");
        string query = $"SELECT * FROM phrases WHERE ROWID = " + select.ToString();
        answer = _db.Query<getPhrase>(query)[0].phrase;
        _db.Close();
        Debug.Log(answer);
        string encrypted = encrypt(answer, offset);
        GetComponent<TextMeshProUGUI>().SetText("Unscramble this phrase: " + encrypted);
    }
   
    string encrypt(string phrase, int offset)
    {
        string cipher = "";
        for (int i = 0; i < phrase.Length; i++)
        {
            if (phrase[i] == ' ')
            {
                cipher += ' ';
                continue;
            }
            cipher += (char)((phrase[i] - 'a' + offset) % 26 + 'a');
        }
        return cipher;
    }

    string decrpyt(string cipher, int offset)
    {
        string plain = "";
        for (int i = 0; i < cipher.Length; i++)
        {
            if (cipher[i] == ' ')
            {
                plain += ' ';
                continue;
            }
            plain += (char)((cipher[i] - 'a' + 26 - offset) % 26 + 'a');
        }
        return plain;
    }

    void submit()
    {
        string choice = inputField.GetComponent<TMP_InputField>().text;
        inputField.GetComponent<TMP_InputField>().text = "";
        if (choice == answer)
        {
            score += 1;
            updateScore();
            startNewQn();
        }
    }

    void gameOver()
    {
        Debug.Log("Game End");
    }

    void showHint()
    {
        string text = "Hint: What place is " + ((char)(offset + 'A')).ToString() + " in the Alphabet?";
        hint.GetComponent<TextMeshProUGUI>().SetText(text);
    }

    void updateScore()
    {
        score_text.GetComponent<TextMeshProUGUI>().SetText(score.ToString());
    }
}
