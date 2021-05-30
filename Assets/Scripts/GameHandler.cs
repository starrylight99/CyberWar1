using System.Collections.Generic;
using UnityEngine;
public class GameHandler : MonoBehaviour {
    private static GameHandler instance;
    [SerializeField] private Transform resourceNode1Transform;
    [SerializeField] private Transform resourceNode2Transform;
    [SerializeField] private Transform resourceNode3Transform;
    [SerializeField] private Transform storageTransform;

    private void Awake() {
        instance = this;
    }

    private Transform GetResourceNode() {
        List<Transform> resourceNodeList = new List<Transform>() { resourceNode1Transform, resourceNode2Transform, resourceNode3Transform };
        return resourceNodeList[UnityEngine.Random.Range(0, resourceNodeList.Count)];
    }

    public static Transform GetResourceNode_Static() {
        return instance.GetResourceNode();
    }
    
    private Transform GetStorage() {
        return storageTransform;
    }
    
    public static Transform GetStorage_Static() {
        return instance.GetStorage();
    }
}