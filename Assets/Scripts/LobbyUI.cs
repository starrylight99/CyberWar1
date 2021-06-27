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
    GameObject settings;

    //[SerializeField] NetworkLobbyManagerCustomised networkLobbyManager;


    public void Host()
    {
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
        //settings = GameObject.Find("SETTINGS");
        //settings.GetComponent<Settings>().playerName = displayName.text;
        //GameObject.FindGameObjectWithTag("NetworkManager").
        //    GetComponent<PlayFabClient>().Authenticate();
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
