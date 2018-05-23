using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {
    //Configured Values
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    public float timeToWallUnstick;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

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
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    void Update() {
        CalculateVelocity();
        HandleWallSliding();
        controller.Move(velocity * Time.deltaTime, directionalInput);

        //Zero out velocity if touching ground
        if(controller.collisions.above || controller.collisions.below) {
            if(controller.collisions.slidingDownMaxSlope) {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            } else {
                velocity.y = 0;
            }
        }
    }

    void HandleWallSliding() {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;

        if((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
            wallSliding = true;

            if(velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }

            if(timeToWallUnstick > 0) {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if(directionalInput.x != wallDirX && directionalInput.x != 0) {
                    timeToWallUnstick -= Time.deltaTime;
                } else {
                    timeToWallUnstick = wallStickTime;
                }
            } else {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    void CalculateVelocity() {
        float targetVelocityX = directionalInput.x * moveSpeed;
        float targetVelocityY = directionalInput.y * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    public void SetDirectionalInput(Vector2 input) {
        directionalInput = input;
    }

    //=========================
    //         Inputs 
    //=========================
    public void OnJumpInputDown() {

        //Wall sliding and jumping
        if(wallSliding) {
            if(wallDirX == directionalInput.x) {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            } else if(directionalInput.x == 0) {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            } else {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }

        //Only jump if colliding with ground
        if(controller.collisions.below) {
            //Check for jumping on slope
            if(controller.collisions.slidingDownMaxSlope) {
                if(directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) { // not jumping against max slope
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            } else {
                velocity.y = maxJumpVelocity;
            }
        }
    }

    public void OnJumpInputUp() {
        if(velocity.y > minJumpVelocity) {
            velocity.y = minJumpVelocity;
        }
    }

    public Vector2 GetVelocity() {
        return velocity;
    }
}
