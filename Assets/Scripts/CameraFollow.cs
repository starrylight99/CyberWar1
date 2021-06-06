using UnityEngine;
using Mirror;

public class CameraFollow : NetworkBehaviour {
    Transform target;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        // change to ClientScene.localPlayer.gameObject when integrating multiplayer
        target = NetworkClient.localPlayer.gameObject.transform;
        Debug.Log(target);
    }

    

    private void Update() {
        if (this.isLocalPlayer)
        {
            Vector3 targetPosition = target.TransformPoint(new Vector3(0, 0, -10));
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        
    }
}