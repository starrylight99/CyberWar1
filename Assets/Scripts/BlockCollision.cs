using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollision : MonoBehaviour
{
    public BoxCollider2D playerCollider;
    public BoxCollider2D playerBlockCollider;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreCollision(playerCollider, playerBlockCollider, true);
    }

    
}
