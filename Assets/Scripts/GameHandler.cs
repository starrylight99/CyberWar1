using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameHandler : MonoBehaviour {
    private static GameHandler instance;
    [SerializeField] private Transform resourceNode1Transform;
    [SerializeField] private Transform resourceNode2Transform;
    [SerializeField] private Transform resourceNode3Transform;
    [SerializeField] private Transform storageTransform;
    [SerializeField] private MiningAI miningAI;
    private List<ResourceNode> resourceNodeList;
    GameObject player;
    private static GameObject objInstance;

    private void Awake() {
        
        
        if (objInstance == null)
        {
            objInstance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        resourceNodeList = new List<ResourceNode>();
        resourceNodeList.Add(new ResourceNode(resourceNode1Transform));
        resourceNodeList.Add(new ResourceNode(resourceNode2Transform));
        resourceNodeList.Add(new ResourceNode(resourceNode3Transform));
        //Debug.Log(resourceNodeList[1].GetPosition());
        ResourceNode.OnResourceNodeClicked += ResourceNode_OnResourceNodeClicked;
    }

    

  

    private ResourceNode GetResourceNode() {
        List<ResourceNode> tmpResourceNodeList = new List<ResourceNode>(resourceNodeList);
        for (int i = 0; i < tmpResourceNodeList.Count; i++) {
            if (!tmpResourceNodeList[i].HasResources()) {
                tmpResourceNodeList.RemoveAt(i);
                i--;
            }
        }
        if (tmpResourceNodeList.Count > 0) {
            return tmpResourceNodeList[UnityEngine.Random.Range(0, tmpResourceNodeList.Count)];
        } else {
            return null;
        }
    }
    private void ResourceNode_OnResourceNodeClicked(object sender, EventArgs e) {
        ResourceNode resourceNode = sender as ResourceNode;
        miningAI.SetResourceNode(resourceNode);
    }
    public static ResourceNode GetResourceNode_Static() {
        return instance.GetResourceNode();
    }
    
    private Transform GetStorage() {
        return storageTransform;
    }
    
    public static Transform GetStorage_Static() {
        return instance.GetStorage();
    }
}