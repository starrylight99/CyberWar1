using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UnitInterface : MonoBehaviour{

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
    }
    private void Update() {
        currSqrMag = (desiredPosition - rb.position).sqrMagnitude;
        if (currSqrMag > sqrMag){
            desiredVelocity = Vector2.zero;
            idle = true;
        }
        sqrMag = currSqrMag;
        //Debug.Log(sqrMag);
        //Debug.Log(currSqrMag);
    }
    private void FixedUpdate() {
        Debug.Log(rb.velocity);
        rb.velocity = desiredVelocity;
    }
    public bool isIdle(){
        return idle;
    }
    public IEnumerator moveTo(Vector2 position, Action onArrivedAtPosition){
        Debug.Log("MoveTo called");
        idle = false;
        desiredPosition = position;
        desiredVelocity = (position - rb.position).normalized * speed;
        sqrMag = (position - rb.position).sqrMagnitude;
        yield return new WaitUntil(() => desiredVelocity == Vector2.zero);
        onArrivedAtPosition();
        idle = true;
    }
    public async void playAnimationMine(Action onAnimationCompleted){
        idle = false;
        await Task.Delay(3000);
        onAnimationCompleted();
        idle = true;
    }
}