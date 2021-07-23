using System.Collections;
using UnityEngine;
using PlayFab;
using System;
using PlayFab.Networking;
using System.Collections.Generic;
using PlayFab.MultiplayerAgent.Model;

public class PlayfabServer : MonoBehaviour {
    private List<ConnectedPlayer> _connectedPlayers;
    public bool Debugging = true;
    public UnityNetworkServer unityNetworkServer;
    // Use this for initialization
    void Start () {
		Configuration configuration = GameObject.FindGameObjectWithTag("Configuration").GetComponent<Configuration>();

        if (configuration.buildType == BuildType.REMOTE_SERVER)
		{
			Debug.Log("Server Initializing");
			_connectedPlayers = new List<ConnectedPlayer>();
			PlayFabMultiplayerAgentAPI.Start();
			PlayFabMultiplayerAgentAPI.IsDebugging = Debugging;
			PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
			PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
			PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;

			unityNetworkServer.OnPlayerAdded.AddListener(OnPlayerAdded);
			unityNetworkServer.OnPlayerRemoved.AddListener(OnPlayerRemoved);

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
        //unityNetworkServer.StartServer();
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
		yield return new WaitForSeconds(300f);
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
