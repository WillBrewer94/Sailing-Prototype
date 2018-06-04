using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Boat : MonoBehaviour {
    //Configured Values
    public float accelerationTimeBase = .2f;
    public float moveSpeed = 6;
    

    //Calculated Values
    Vector3 velocity;
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    bool wallSliding;
    int wallDirX;
    float velocityXSmoothing;
    float velocityYSmoothing;

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
        float targetVelocityX = directionalInput.x * moveSpeed;
        float targetVelocityY = directionalInput.y * moveSpeed;
        //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeBase);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, accelerationTimeBase);
        transform.Rotate(Vector3.back * Time.deltaTime * targetVelocityX * 20);
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