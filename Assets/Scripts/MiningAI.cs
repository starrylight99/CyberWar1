using UnityEngine;

public class MiningAI : MonoBehaviour {
    private enum State {
        Idle,
        MovingToResourceNode,
        GatheringResources,
        MovingToStorage,
    }

    private UnitInterface unit;
    private State state;
    private Transform resourceNodeTransform;
    private Transform storageTransform;
    private int goldInventoryAmount;

    private void Awake() {
        unit = gameObject.GetComponent<UnitInterface>();
        state = State.Idle;
    }

    private void Update() {
        Debug.Log(state);
        switch (state) {
        case State.Idle:
            resourceNodeTransform = GameHandler.GetResourceNode_Static();
            //Debug.Log(resourceNodeTransform.position);
            //Debug.Log(unit.transform.position);
            state = State.MovingToResourceNode;
            break;
        case State.MovingToResourceNode:
            if (unit.isIdle()) {
                Debug.Log(unit.isIdle());
                unit.moveTo(resourceNodeTransform.position, () => {
                    state = State.GatheringResources;
                });
            }
            break;
        case State.GatheringResources:
            if (unit.isIdle()) {
                if (goldInventoryAmount > 0) {
                    // Move to storage
                    storageTransform = GameHandler.GetStorage_Static();
                    state = State.MovingToStorage;
                } else {
                    // Gather resources
                    unit.playAnimationMine(() => {
                        goldInventoryAmount++;
                    });
                }
            }
            break;
        case State.MovingToStorage:
            if (unit.isIdle()) {
                unit.moveTo(storageTransform.position, () => {
                    GameResources.AddGoldAmount(goldInventoryAmount);
                    goldInventoryAmount = 0;
                    state = State.Idle;
                });
            }
            break;
        }
    }
}
