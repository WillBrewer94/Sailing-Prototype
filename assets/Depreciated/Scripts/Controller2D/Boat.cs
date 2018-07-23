using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Boat : MonoBehaviour {
    //Configured Values
    public float accelerationTimeBase = 1f;
    public float rotTimeBase = 1f;
    public float moveSpeed = 100;
    public float rotSpeed = 100;
    

    //Calculated Values
    Vector3 velocity;
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    bool wallSliding;
    int wallDirX;
    float velocityXSmoothing;
    float velocityYSmoothing;
    float rotVelSmoothing;
    float rotVel;

    //Components
    Controller2D controller;
    SpriteRenderer sprt;
    Animator anim;

    //Input
    Vector2 directionalInput;

    void Start() {
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
        sprt = GetComponent<SpriteRenderer>();

        //Calculate movement values based on configuration
    }

    void Update() {
        CalculateVelocity();
        controller.Move(velocity * Time.deltaTime, directionalInput);
    }


    void CalculateVelocity() {
        float targetRotSpeed = directionalInput.x * rotSpeed;
        float targetVelocityY = directionalInput.y * moveSpeed;
        //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeBase);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, accelerationTimeBase);
        rotVel = Mathf.SmoothDamp(rotVel, targetRotSpeed, ref rotVelSmoothing, rotTimeBase);
        transform.Rotate(Vector3.back * Time.deltaTime * rotVel);
    }

    public void SetDirectionalInput(Vector2 input) {
        directionalInput = input;
    }

    //=========================
    //         Inputs 
    //=========================

    public Vector2 GetVelocity() {
        return velocity;
    }
}