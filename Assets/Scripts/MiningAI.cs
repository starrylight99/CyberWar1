using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class MiningAI : MonoBehaviour {
    private enum State {
        Idle,
        MovingToResourceNode,
        GatheringResources,
        MovingToStorage,
    }
    private State state;
    private ResourceNode resourceNode;
    private Transform storageTransform;
    private int resourceInventoryAmount;
    bool idle;
    Rigidbody2D rb;
    public float speed = 15;
    Vector2 desiredVelocity,desiredPosition;
    float sqrMag,currSqrMag;
    TextMesh inventoryTextMesh;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        idle = true;
        desiredPosition = rb.transform.position;
        desiredVelocity = Vector2.zero;
        state = State.Idle;
        inventoryTextMesh = transform.Find("InventoryTextMesh").GetComponent<TextMesh>();
        UpdateInventoryText();
    }

    private async void Update() {
        Debug.Log(state);
        currSqrMag = (desiredPosition - rb.position).sqrMagnitude;
        if (currSqrMag > sqrMag){
            desiredVelocity = Vector2.zero;
        }
        sqrMag = currSqrMag;
        switch (state) {
        case State.Idle:
            //resourceNode = GameHandler.GetResourceNode_Static();
            if (resourceNode != null) {
                state = State.MovingToResourceNode;
            }
            break;
        case State.MovingToResourceNode:
            if (idle) {
                StartCoroutine(moveTo(resourceNode.GetPosition(), () => {
                    state = State.GatheringResources;
                }));
            }
            break;
        case State.GatheringResources:
            if (idle) {
                if (resourceInventoryAmount >= 3) {
                    storageTransform = GameHandler.GetStorage_Static();
                    state = State.MovingToStorage;
                } else {
                    await playAnimationMine(() => {
                        resourceNode.GrabResource();
                        resourceInventoryAmount++;
                        UpdateInventoryText();
                    });
                }
            }
            break;
        case State.MovingToStorage:
            if (idle) {
                StartCoroutine(moveTo(storageTransform.position, () => {
                    GameResources.AddGoldAmount(resourceInventoryAmount);
                    Debug.Log(GameResources.GetGoldAmount());
                    resourceInventoryAmount = 0;
                    UpdateInventoryText();
                    state = State.Idle;
                }));
            }
            break;
        }
    }
    private void FixedUpdate() {
        rb.velocity = desiredVelocity;
    }
    public bool isIdle(){
        return idle;
    }
    public IEnumerator moveTo(Vector2 position, Action onArrivedAtPosition){
        idle = false;
        desiredPosition = position;
        desiredVelocity = (position - rb.position).normalized * speed;
        sqrMag = (position - rb.position).sqrMagnitude;
        yield return new WaitUntil(() => desiredVelocity == Vector2.zero);
        onArrivedAtPosition();
        idle = true;
    }
    public async Task playAnimationMine(Action onAnimationCompleted){
        idle = false;
        await Task.Delay(1000);
        onAnimationCompleted();
        idle = true;
    }
    private void UpdateInventoryText() {
        if (resourceInventoryAmount > 0) {
            inventoryTextMesh.text = "" + resourceInventoryAmount;
        } else {
            inventoryTextMesh.text = "";
        }
    }
    public void SetResourceNode(ResourceNode resourceNode) {
        this.resourceNode = resourceNode;
    }
}
