using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;

public class Move : NetworkBehaviour {

    
    public float speed = 15;
    Vector2 velocity;
    Rigidbody2D rb;
    Animator animator;
    bool isMoving;
    [SyncVar]
    public bool scramble;
    public bool scrambleSet;
    private static GameObject instance;
    private bool swapXY = false;
    private bool revX = false;
    private bool revY = false;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (this.isLocalPlayer)
        {
            Debug.Log("Client Started");
            if (instance == null)
            {
                instance = gameObject;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
                return;
            }
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            velocity = Vector2.zero;
            isMoving = false;
            transform.GetChild(0).gameObject.SetActive(true);
        } 
    }

    private void Update() {
        if (this.isLocalPlayer)
        {
            velocity.x = Input.GetAxisRaw("Horizontal");
            velocity.y = Input.GetAxisRaw("Vertical");

            if (scramble && !scrambleSet)
            {
                swapXY = Random.value > 0.5;
                revX = Random.value > 0.5;
                revY = Random.value > 0.5;
                Debug.Log(string.Format("swapXY: {0} revX: {1} revY: {2}", swapXY, revX, revY));
                scrambleSet = true;
                StartCoroutine(StopScrambling());
            }
            else if (!scramble)
            {
                swapXY = false;
                revX = false;
                revY = false;
                scrambleSet = false;
            }

            if (swapXY)
            {
                float temp = velocity.x;
                velocity.x = velocity.y;
                velocity.y = temp;
            }
            if (revX)
            {
                velocity.x = -velocity.x;
            }
            if (revY)
            {
                velocity.y = -velocity.y;
            }

            if (velocity != Vector2.zero)
            {
                isMoving = true;
                animator.SetFloat("moveX", velocity.x);
                animator.SetFloat("moveY", velocity.y);
                animator.SetBool("isMoving", isMoving);
            }
            else
            {
                isMoving = false;
                animator.SetBool("isMoving", isMoving);
            }
        }
    }

    IEnumerator StopScrambling()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("Stop Scrambling");
        CmdStopScrambling(gameObject);
    }

    [Command]
    void CmdStopScrambling(GameObject player)
    {
        player.GetComponent<Move>().scramble = false;
    }

    private void FixedUpdate() {
        if (this.isLocalPlayer)
        {
            rb.MovePosition(rb.position + velocity * speed * Time.fixedDeltaTime);
        }
        
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Client Stopped");
        
    }

}