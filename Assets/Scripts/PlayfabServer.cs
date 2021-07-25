using System.Collections;
using UnityEngine;
using PlayFab;
using System;
using PlayFab.Networking;
using System.Collections.Generic;
using PlayFab.MultiplayerAgent.Model;

public class PlayfabServer : MonoBehaviour {
    private List<ConnectedPlayer> _connectedPlayers;
    public static PlayfabServer Instance { get; private set; }
    public bool Debugging = true;
    // Use this for initialization
    void Start () {
		Configuration configuration = GameObject.FindGameObjectWithTag("Configuration").GetComponent<Configuration>();

        if (configuration.buildType == BuildType.REMOTE_SERVER)
		{
			Debug.Log("Server Initializing");
            Instance = this;
            DontDestroyOnLoad(Instance);
			_connectedPlayers = new List<ConnectedPlayer>();
			PlayFabMultiplayerAgentAPI.Start();
			PlayFabMultiplayerAgentAPI.IsDebugging = Debugging;
			PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
			PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
			PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;

			NetworkLobbyManagerCustomised.Instance.OnPlayerAdded.AddListener(OnPlayerAdded);
			NetworkLobbyManagerCustomised.Instance.OnPlayerRemoved.AddListener(OnPlayerRemoved);

			StartCoroutine(ReadyForPlayers());
            StartCoroutine(ShutdownServerInXTime());
		}
    }

    IEnumerator ReadyForPlayers()
    {
        yield return new WaitForSeconds(.5f);
        PlayFabMultiplayerAgentAPI.ReadyForPlayers();
    }
    
    private void OnServerActive()
    {
        NetworkLobbyManagerCustomised.Instance.StartListen();
        Debug.Log("Server Started From Agent Activation");
    }

    private void OnPlayerRemoved(string playfabId)
    {
        ConnectedPlayer player = _connectedPlayers.Find(x => x.PlayerId.Equals(playfabId, StringComparison.OrdinalIgnoreCase));
        _connectedPlayers.Remove(player);
        PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
    }

    private void OnPlayerAdded(string playfabId)
    {
        Debug.Log("OnPlayerAdded Invoked. PlayfabId: " + playfabId);
        _connectedPlayers.Add(new ConnectedPlayer(playfabId));
        PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
    }

    private void OnAgentError(string error)
    {
        Debug.Log(error);
    }

    private void OnShutdown()
    {
        StartShutdownProcess();
    }    
    IEnumerator ShutdownServerInXTime()
	{
		yield return new WaitForSeconds(450f);
		StartShutdownProcess();
	}
    private void StartShutdownProcess()
	{
		Debug.Log("Server is shutting down");
		StartCoroutine(ShutdownServer());
	}
    IEnumerator ShutdownServer()
	{
		yield return new WaitForSeconds(5f);
		Application.Quit();
	}
}
