using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Move : NetworkBehaviour {

    
    public float speed = 15;
    Vector2 velocity;
    Rigidbody2D rb;
    Animator animator;
    bool isMoving;
    private static GameObject instance;

    private void Awake() {
        Debug.Log("awake");
    }

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

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //    }
    //}
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //    }
    //}
}