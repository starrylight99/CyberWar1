using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using System;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using Mirror;
using PlayFab.Networking;
using PlayFab.Helpers;
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
    Configuration configuration;
    NetworkLobbyManagerCustomised networkManager;
    TelepathyTransport telepathyTransport;
    bool host;
    String playFabId;
    PlayFabAuthService _authService;
    string ip,port,roomName;
    void Start()
    {
        _authService = PlayFabAuthService.Instance;
        PlayFabAuthService.OnLoginSuccess += OnLoginSuccess;
        PlayFabAuthService.OnDisplayAuthentication += OnDisplayAuth;
    }
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

        configuration = GameObject.FindGameObjectWithTag("Configuration").GetComponent<Configuration>();
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkLobbyManagerCustomised>();
        telepathyTransport = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<TelepathyTransport>();

        //networkManager.OnConnected.AddListener(OnConnected);

        if (configuration.buildType == BuildType.REMOTE_CLIENT)
		{
			if (configuration.buildId == "")
			{
				throw new Exception("Remote client build must have a buildId");
			}
			else
			{
				LoginRemoteUser();
                host = true;
			}
		}
		else if (configuration.buildType == BuildType.LOCAL_CLIENT)
		{
			networkManager.StartHost();
		}
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

        configuration = GameObject.FindGameObjectWithTag("Configuration").GetComponent<Configuration>();
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkLobbyManagerCustomised>();
        telepathyTransport = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<TelepathyTransport>();

        //networkManager.OnConnected.AddListener(OnConnected);

        if (configuration.buildType == BuildType.REMOTE_CLIENT)
		{
			if (configuration.buildId == "")
			{
				throw new Exception("Remote client build must have a buildId");
			}
			else
			{
                networkManager.networkAddress = joinMatchInput.text;
				LoginRemoteUser();
                host = false;
			}
		}
		else if (configuration.buildType == BuildType.LOCAL_CLIENT)
		{
            networkManager.networkAddress = joinMatchInput.text;
			networkManager.StartClient();
		}
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

    public void LoginRemoteUser()
	{
		Debug.Log("[Client].LoginRemoteUser");
		
		//We need to login a user to get at PlayFab API's. 
		LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
		{
			TitleId = PlayFabSettings.TitleId,
			CreateAccount = true,
			CustomId = displayName.text
		};

		PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnLoginError);
	}

    private void OnLoginError(PlayFabError response)
	{
        Debug.Log("[Client] Login Failure");
		Debug.Log(response.ToString());
	}

	private void OnPlayFabLoginSuccess(LoginResult response)
	{
		Debug.Log("[Client] Login Success");
        playFabId = response.PlayFabId;
		if (host/* configuration.ipAddress == "" */)
		{   //We need to grab an IP and Port from a server based on the buildId. Copy this and add it to your Configuration.
            RequestMultiplayerServer();
		}
		else
		{
			ConnectRemoteClient();
		}
	}

	private void RequestMultiplayerServer()
	{
		Debug.Log("[Client].RequestMultiplayerServer");
		RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest();
		requestData.BuildId = configuration.buildId;
		requestData.SessionId = System.Guid.NewGuid().ToString();
		requestData.PreferredRegions = new List<string>() { "EastUs" };
		PlayFabMultiplayerAPI.RequestMultiplayerServer(requestData, OnRequestMultiplayerServer, OnRequestMultiplayerServerError);
	}

	private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
	{
		Debug.Log(response.ToString());
		ConnectRemoteClient(response);
	}

	private void ConnectRemoteClient(RequestMultiplayerServerResponse response = null)
	{
        roomName = GameObject.FindGameObjectWithTag("MatchId").GetComponent<TMP_InputField>().text;
        Debug.Log(roomName);
		if(response == null) 
		{
            GetRoomDetails();
		}
		else
		{
			Debug.Log("IP: " + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num);
            ip = response.IPV4Address;
            ushort portInt = (ushort)response.Ports[0].Num;
            port = portInt.ToString();
			networkManager.networkAddress = response.IPV4Address;
			telepathyTransport.port = (ushort)response.Ports[0].Num;
            _authService.Authenticate();
        }
	}

	private void OnRequestMultiplayerServerError(PlayFabError error)
	{
        Debug.Log("[Client].OnRequestMultiplayerServerError" + error);
		Debug.Log(error.ErrorDetails);
	}

    private void OnConnected()
    {
        Debug.Log("Playfab Authenticating");
        _authService.Authenticate();
    }

    private void OnDisplayAuth()
    {
        _authService.Authenticate(Authtypes.Silent);
    }

    private void OnLoginSuccess(LoginResult success)
    {
        Debug.Log("Playfab Auth success");
        networkManager.StartClient();
        StartCoroutine(OnClientConnect());
    }

    private IEnumerator OnClientConnect(){
        while(!NetworkClient.isConnected){
            Debug.Log("Network Client is connected: " + NetworkClient.isConnected);
            yield return new WaitForSeconds(1);
        }
        Debug.Log(NetworkClient.isConnected);
        NetworkClient.connection.Send<ReceiveAuthenticateMessage>(new ReceiveAuthenticateMessage()
        {
            PlayFabId = playFabId
        });
        Debug.Log("Authenticate message sent");
        
        if (host){
            CreateRoom();
        }
    }
    //Create Room
    private void CreateRoom(){
        CreateSharedGroupRequest request = new CreateSharedGroupRequest(){
            SharedGroupId = roomName
        };
        PlayFabClientAPI.CreateSharedGroup(request,OnCreateRoomSuccess,OnCreateRoomFailed);
    }
    private void OnCreateRoomSuccess(CreateSharedGroupResult result){
        Debug.Log("Create Group Success");
        Debug.Log(result);
        UpdateRoom();
    }
    private void OnCreateRoomFailed(PlayFabError error){
        Debug.LogWarning("Create Group Failed");
        Debug.Log(error.ErrorDetails);
    }
    //Update group details
    private void UpdateRoom(){
        Dictionary<string,string> data = new Dictionary<string,string>(){
            {"ip",ip},
            {"port",port}
        };
        UpdateSharedGroupDataRequest request = new UpdateSharedGroupDataRequest(){
            SharedGroupId = roomName,
            Data = data,
            //KeysToRemove = [],
            Permission = UserDataPermission.Public
            //CustomTags = null
        };
        PlayFabClientAPI.UpdateSharedGroupData(request,OnUpdateRoomSuccess,OnUpdateRoomFailed);
    }
    private void OnUpdateRoomSuccess(UpdateSharedGroupDataResult result){
        Debug.Log("Update Details Success");
        Debug.Log(result);
    }
    private void OnUpdateRoomFailed(PlayFabError error){
        Debug.LogWarning("Update Details Failed");
        Debug.Log(error.ErrorDetails);
        Debug.Log(error.Error);
        Debug.Log(error.ErrorMessage);
        Debug.Log(error.HttpCode);
        foreach(KeyValuePair<string, List<String>> data in error.ErrorDetails) {
            Debug.Log(data.Key);
            foreach (var item in data.Value)
            {   
                Debug.Log(item);
            }
        }
    }

    // Get Room details
    private void GetRoomDetails(){
        GetSharedGroupDataRequest request = new GetSharedGroupDataRequest(){
            SharedGroupId = roomName
        };
        PlayFabClientAPI.GetSharedGroupData(request,OnGetRoomDetailsSuccess,OnGetRoomDetailsFailed);
    }
    private void OnGetRoomDetailsSuccess(GetSharedGroupDataResult result){
        Debug.Log("Get Group Success");
        Debug.Log(result.Data);
        foreach(KeyValuePair<string, SharedGroupDataRecord> data in result.Data) {
            if (data.Key == "ip"){
                networkManager.networkAddress = data.Value.Value;
            } else if (data.Key == "port"){
                telepathyTransport.port = Convert.ToUInt16(data.Value.Value);
            }
            Debug.Log(data.Key + " : " + data.Value.Value);
        }
        _authService.Authenticate();
    }
    private void OnGetRoomDetailsFailed(PlayFabError error){
        Debug.LogWarning("Get Group Failed");
        Debug.Log(error);
    }
}  