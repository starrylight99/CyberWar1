using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;

public class MiningAI : NetworkBehaviour {
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
    public bool sabotaged = false;
    public bool isAttack;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        idle = true;
        desiredPosition = rb.transform.position;
        desiredVelocity = Vector2.zero;
        state = State.Idle;
        inventoryTextMesh = transform.Find("InventoryTextMesh").GetComponent<TextMesh>();
        UpdateInventoryText();
    }

    async void Update() {
        //Debug.Log(state);
        currSqrMag = (desiredPosition - rb.position).sqrMagnitude;
        if (currSqrMag > sqrMag){
            desiredVelocity = Vector2.zero;
        }
        sqrMag = currSqrMag;
        if (sabotaged)
        {
            state = State.Idle;
        }
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
                    GameObject[] arr = GameObject.FindGameObjectsWithTag("GameHandler");
                    foreach (GameObject gh in arr)
                    {
                        if (isAttack == gh.GetComponent<TeamTag>().isAttack){
                            storageTransform = gh.GetComponent<GameHandler>().GetStorage();
                        }
                    }
                    //storageTransform = GameHandler.GetStorage_Static();
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
                //storageTransform = GameHandler.GetStorage_Static();

                StartCoroutine(moveTo(storageTransform.position, () => {
                    AddGoldAmountServer(resourceInventoryAmount, isAttack);
                    //GameResources.AddGoldAmount(resourceInventoryAmount, isAttack);
                    resourceInventoryAmount = 0;
                    UpdateInventoryText();
                    state = State.Idle;
                }));
            }
            break;
        }
    }
    void FixedUpdate() {
        rb.velocity = desiredVelocity;
    }
    public bool isIdle(){
        return idle;
    }
    public IEnumerator moveTo(Vector2 position, Action onArrivedAtPosition){
        idle = false;
        desiredPosition = position;
        desiredVelocity = (position - rb.position).normalized * speed;
        Vector2 dir = (new Vector2(transform.position.x, transform.position.y) - position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI;
        angle = ((dir.x < 0) && (dir.y < 0)) ? angle += 270 : angle -= 90;
        transform.eulerAngles = new Vector3(0, 0, angle);
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
        try //not sure what else i can do about this
        {
            if (resourceInventoryAmount > 0)
            {
                inventoryTextMesh.text = "" + resourceInventoryAmount;
            }
            else
            {
                inventoryTextMesh.text = "";
            }
        } catch (MissingReferenceException)
        {
            Debug.Log("Destroyed");
        } 
    }
    public void SetResourceNode(ResourceNode resourceNode) {
        this.resourceNode = resourceNode;
    }

    [Server]
    private void AddGoldAmountServer(int value, bool isAtk){
        GameResources.AddGoldAmount(value, isAttack);
    }
}
