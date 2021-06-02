using UnityEngine;

public class Move : MonoBehaviour {
    

    public float speed = 15;
    Vector2 velocity;
    Rigidbody2D rb;
    Animator animator;
    bool isMoving;
    private static GameObject instance;

    private void Awake() {
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
    }

    private void Update() {
        velocity.x = Input.GetAxisRaw("Horizontal");
        velocity.y = Input.GetAxisRaw("Vertical");

        if (velocity != Vector2.zero){
            isMoving = true;
            animator.SetFloat("moveX", velocity.x);
            animator.SetFloat("moveY", velocity.y);
            animator.SetBool("isMoving", isMoving);
        } else {
            isMoving = false;
            animator.SetBool("isMoving", isMoving);
        }
    }

    private void FixedUpdate() {
        rb.MovePosition(rb.position + velocity * speed * Time.fixedDeltaTime);
    }
}