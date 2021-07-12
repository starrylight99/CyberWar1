using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] TMP_InputField displayName;
    [SerializeField] TMP_InputField joinMatchInput;
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;
    [SerializeField] GameObject message;
    public bool isAttack;
    public bool choseTeam = false;
    string[] vulgarities = { "stupid", "idiot", "fuck", "shit" };
    public GameObject accept;
    public GameObject reject;
    private bool accepted = true;

    private void Update()
    {
        if (displayName.text.Length != 0)
        {
            bool acceptName = true;
            foreach (string vulgarity in vulgarities)
            {
                if (displayName.text.Contains(vulgarity))
                {
                    if (!reject.activeInHierarchy)
                    {
                        accept.SetActive(false);
                        reject.SetActive(true);
                        
                    }
                    acceptName = false;
                    accepted = false;
                }
            }
            if (acceptName && !accept.activeInHierarchy)
            {
                accept.SetActive(true);
                reject.SetActive(false);
                accepted = true;
            }
        }
        else
        {
            accept.SetActive(false);
            reject.SetActive(false);
        }
    }


    public void Host()
    {
        if (!accepted)
        {
            message.GetComponent<TextMeshProUGUI>().SetText("Please Choose an appropriate name!");
            StartCoroutine(removeText());
            return;
        }
        if (!choseTeam)
        {
            message.GetComponent<TextMeshProUGUI>().SetText("Please Choose your team first!");
            StartCoroutine(removeText());
            return;
        }
        displayName.interactable = false;
        joinMatchInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;
        GameObject.FindGameObjectWithTag("NetworkManager").
            GetComponent<NetworkLobbyManagerCustomised>().StartHost();
    }   
    public void Join()
    {
        if (!accepted)
        {
            message.GetComponent<TextMeshProUGUI>().SetText("Please Choose an appropriate name!");
            StartCoroutine(removeText());
            return;
        }
        if (!choseTeam)
        {
            message.GetComponent<TextMeshProUGUI>().SetText("Please Choose your team first!");
            StartCoroutine(removeText());
            return;
        }
        displayName.interactable = false;
        joinMatchInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;
        GameObject.FindGameObjectWithTag("NetworkManager").
            GetComponent<NetworkLobbyManagerCustomised>().networkAddress = joinMatchInput.text;
        GameObject.FindGameObjectWithTag("NetworkManager").
            GetComponent<NetworkLobbyManagerCustomised>().StartClient();
    }

    public void Attack()
    {
        isAttack = true;
        choseTeam = true;
    }

    public void Defend()
    {
        isAttack = false;
        choseTeam = true;
    }

    private IEnumerator removeText()
    {
        yield return new WaitForSeconds(5f);
        if (transform.GetChild(0).gameObject.activeInHierarchy)
        {
            message.GetComponent<TextMeshProUGUI>().SetText("");
        }
    }
}
