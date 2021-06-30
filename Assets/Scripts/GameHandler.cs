using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Mirror;
using static ResourceNode;

public class GameHandler : NetworkBehaviour {
    private static GameHandler instance;
    [SerializeField] private Transform resourceNode1Transform;
    [SerializeField] private Transform resourceNode2Transform;
    [SerializeField] private Transform resourceNode3Transform;
    [SerializeField] private Transform storageTransform;
    [SerializeField] private MiningAI miningAI;
    public List<ResourceNode> resourceNodeList;
    States player;
    private static GameObject objInstance;

    private void Awake() {
        instance = this;
        resourceNodeList = new List<ResourceNode>();
        resourceNodeList.Add(new ResourceNode(resourceNode1Transform,0));
        resourceNodeList.Add(new ResourceNode(resourceNode2Transform,1));
        resourceNodeList.Add(new ResourceNode(resourceNode3Transform,2));
        //Debug.Log(resourceNodeList[1].GetPosition());
        //ResourceNode.OnResourceNodeClicked += ResourceNode_OnResourceNodeClicked;
        ResourceNode.OnResourceNodeClicked += GetPlayerResourceNode;
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
    private void GetPlayerResourceNode(object sender, EventArgs e){
        ResourceNode resourceNode = sender as ResourceNode;
        player = NetworkClient.localPlayer.gameObject.GetComponent<States>();
        player.SetResourceNodeServer(resourceNode.serialNumber, player.isAttack);
    }
    /* public void ResourceNode_OnResourceNodeClicked(object sender, EventArgs e) {
        ResourceNode resourceNode = sender as ResourceNode;
        miningAI.SetResourceNode(resourceNode);
    } */
    public static ResourceNode GetResourceNode_Static() {
        return instance.GetResourceNode();
    }
    
    public Transform GetStorage() {
        return storageTransform;
    }
    
    public static Transform GetStorage_Static() {
        return instance.GetStorage();
    }
}