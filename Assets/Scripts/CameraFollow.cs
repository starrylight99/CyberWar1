using UnityEngine;

public class CameraFollow : MonoBehaviour {
    Transform target;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        // change to ClientScene.localPlayer.gameObject when integrating multiplayer
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        Vector3 targetPosition = target.TransformPoint(new Vector3(0,0,-10));
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}