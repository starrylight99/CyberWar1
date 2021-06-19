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
    [SerializeField] NetworkLobbyManagerCustomised networkLobbyManager;
    public void Host()
    {
        displayName.interactable = false;
        joinMatchInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;
        networkLobbyManager.StartHost();
    }   
    public void Join()
    {
        displayName.interactable = false;
        joinMatchInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;
        networkLobbyManager.StartClient();
        networkLobbyManager.networkAddress = joinMatchInput.text;
    }
}
