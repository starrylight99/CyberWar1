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
    private Transform resourceNodeTransform;
    private Transform storageTransform;
    private int resourceInventoryAmount;
    bool idle;
    Rigidbody2D rb;
    public float speed = 15;
    Vector2 desiredVelocity,desiredPosition;
    float sqrMag,currSqrMag;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        idle = true;
        desiredPosition = rb.transform.position;
        desiredVelocity = Vector2.zero;
        state = State.Idle;
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
            resourceNodeTransform = GameHandler.GetResourceNode_Static();
            state = State.MovingToResourceNode;
            break;
        case State.MovingToResourceNode:
            if (idle) {
                StartCoroutine(moveTo(resourceNodeTransform.position, () => {
                    state = State.GatheringResources;
                }));
            }
            break;
        case State.GatheringResources:
            if (idle) {
                if (resourceInventoryAmount > 0) {
                    storageTransform = GameHandler.GetStorage_Static();
                    state = State.MovingToStorage;
                } else {
                    await playAnimationMine(() => {
                        resourceInventoryAmount++;
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
        await Task.Delay(3000);
        onAnimationCompleted();
        idle = true;
    }
}
