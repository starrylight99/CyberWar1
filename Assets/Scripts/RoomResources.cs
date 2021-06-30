using UnityEngine;
using UnityEngine.Networking;
using Mirror;

public class RoomResources : NetworkBehaviour {
    // Obsolete for now. Resources currently stored in GameResources
    [SyncVar]
    public int atkTeamResources,defTeamResources;

    private void Awake() {
        atkTeamResources = 0;
        defTeamResources = 0;
    }
}