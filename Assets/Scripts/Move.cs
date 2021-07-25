using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

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
    public bool slowed;
    private bool alrSlowed;
    public bool canMove = true;
    public float slowPercent;
    public float confusedDuration;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (this.isLocalPlayer)
        {
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
        if (this.isLocalPlayer && canMove)
        {
            if (SceneManager.GetActiveScene().name.Contains("FinalBattle"))
            {
                if (slowed && !alrSlowed)
                {
                    speed *= slowPercent;
                    alrSlowed = true;
                    TextMeshProUGUI TMPText = GameObject.FindGameObjectWithTag("UI").
                                    transform.GetChild(5).GetComponent<TextMeshProUGUI>();
                    TMPText.SetText(TMPText.text + "Slowed!\n");
                }
                else if (!slowed && alrSlowed)
                {
                    alrSlowed = false;
                    speed *= 1 / slowPercent;
                    TextMeshProUGUI TMPText = GameObject.FindGameObjectWithTag("UI").
                                    transform.GetChild(5).GetComponent<TextMeshProUGUI>();
                    string newText = TMPText.text.Replace("Slowed!\n", "");
                    TMPText.SetText(newText);
                }
            }
            velocity.x = Input.GetAxisRaw("Horizontal");
            velocity.y = Input.GetAxisRaw("Vertical");

            if (scramble && !scrambleSet)
            {
                do
                {
                    swapXY = Random.value > 0.5;
                    revX = Random.value > 0.5;
                    revY = Random.value > 0.5;
                } while (!swapXY && !revX && !revY);
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
        TextMeshProUGUI TMPText = GameObject.FindGameObjectWithTag("UI").
            transform.GetChild(5).GetComponent<TextMeshProUGUI>();
        TMPText.SetText(TMPText.text + "Confused!\n");
        yield return new WaitForSeconds(confusedDuration);
        string newText = TMPText.text.Replace("Confused!\n", "");
        TMPText.SetText(newText);
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
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}