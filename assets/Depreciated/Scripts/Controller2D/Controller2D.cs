using UnityEngine;
using System;
using System.Collections.Generic;

public class Controller2D : RaycastController {
    public float maxSlopeAngle = 80;

    [HideInInspector]
    public Vector2 playerInput;
    public RaycastHit2D hit;
    public List<RaycastHit2D> raycastHitsThisFrame = new List<RaycastHit2D>(2);

    public CollisionInfo collisions;
    public event Action<RaycastHit2D> onControllerCollidedEvent;
    public event Action<Collider2D> onTriggerEnterEvent;
    public event Action<Collider2D> onTriggerStayEvent;
    public event Action<Collider2D> onTriggerExitEvent;

    public override void Start() {
        base.Start();
        collisions.faceDir = 1;
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if(onTriggerEnterEvent != null) {
            onTriggerEnterEvent(col);
        }
    }


    public void OnTriggerStay2D(Collider2D col) {
        if(onTriggerStayEvent != null) {
            onTriggerStayEvent(col);
        }
    }


    public void OnTriggerExit2D(Collider2D col) {
        if(onTriggerExitEvent != null) {
            onTriggerExitEvent(col);
        }
    }

    //Alternate move method for when no input is specified
    public void Move(Vector2 deltaMove, bool standingOnPlatform) {
        Move(deltaMove, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 deltaMove, Vector2 input, bool standingOnPlatform = false) {
        //Clear state
        collisions.Reset();

        UpdateRaycastOrigins();

        //Grab directional input
        playerInput = input;

        //Descend slope if only moving down
        if(deltaMove.y < 0) {
            DescendSlope(ref deltaMove);
        }

        //Check for horizontal collisions
        if(deltaMove.x != 0) {
            HorizontalCollisions(ref deltaMove);
        }
        
        //Check for vertical collisions
        if(deltaMove.y != 0) {
            VerticalCollisions(ref deltaMove);
        }

        transform.Translate(deltaMove);

        if(standingOnPlatform) {
            collisions.below = true;
        }

        if(onControllerCollidedEvent != null) {
            for(var i = 0; i < raycastHitsThisFrame.Count; i++) {
                onControllerCollidedEvent(raycastHitsThisFrame[i]);
            }
        }
    }

    void HorizontalCollisions(ref Vector2 deltaMove) {
        int directionX = (int)Mathf.Sign(deltaMove.x);
        float rayLength = Mathf.Abs(deltaMove.x) + skinWidth;

        //Makes sure no rays are drawn shorter than the skin width
        if(Mathf.Abs(deltaMove.x) < skinWidth) {
            rayLength = 2 * skinWidth;
        }

        for(int i = 0; i < horizontalRayCount; i++) {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if(hit) {
                if(hit.distance == 0) {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                //Only the bottom ray can hit slopes, so there is a special case for it
                if(i == 0 && slopeAngle <= maxSlopeAngle) {
                    if(collisions.descendingSlope) {
                        collisions.descendingSlope = false;
                    }

                    float distanceToSlopeStart = 0;
                    if(slopeAngle != collisions.slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        deltaMove.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref deltaMove, slopeAngle, hit.normal);
                    deltaMove.x += distanceToSlopeStart * directionX;
                }

                if(!collisions.climbingSlope || slopeAngle > maxSlopeAngle) {
                    deltaMove.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if(collisions.climbingSlope) {
                        deltaMove.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaMove.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }

                raycastHitsThisFrame.Add(hit);
            }
        }
    }

    void VerticalCollisions(ref Vector2 deltaMove) {
        int directionY = (int)Mathf.Sign(deltaMove.y);
        float rayLength = Mathf.Abs(deltaMove.y) + skinWidth;

        for(int i = 0; i < verticalRayCount; i++) {

            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + deltaMove.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if(hit) {
                if(hit.collider.tag == "Through") {
                    if(directionY == 1 || hit.distance == 0) {
                        continue;
                    }
                    if(collisions.fallingThroughPlatform) {
                        continue;
                    }
                    if(playerInput.y == -1) {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .5f);
                        continue;
                    }
                }

                deltaMove.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if(collisions.climbingSlope) {
                    deltaMove.x = deltaMove.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(deltaMove.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;

                raycastHitsThisFrame.Add(hit);
            }
        }

        if(collisions.climbingSlope) {
            float directionX = Mathf.Sign(deltaMove.x);
            rayLength = Mathf.Abs(deltaMove.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * deltaMove.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if(hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collisions.slopeAngle) {
                    deltaMove.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 deltaMove, float slopeAngle, Vector2 slopeNormal) {
        float moveDistance = Mathf.Abs(deltaMove.x);
        float climbDeltaMoveY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if(deltaMove.y <= climbDeltaMoveY) {
            deltaMove.y = climbDeltaMoveY;
            deltaMove.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(deltaMove.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    void DescendSlope(ref Vector2 deltaMove) {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(deltaMove.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(deltaMove.y) + skinWidth, collisionMask);
        if(maxSlopeHitLeft ^ maxSlopeHitRight) {
            SlideDownMaxSlope(maxSlopeHitLeft, ref deltaMove);
            SlideDownMaxSlope(maxSlopeHitRight, ref deltaMove);
        }

        if(!collisions.slidingDownMaxSlope) {
            float directionX = Mathf.Sign(deltaMove.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if(hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != 0 && slopeAngle <= maxSlopeAngle) {
                    if(Mathf.Sign(hit.normal.x) == directionX) {
                        if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaMove.x)) {
                            float moveDistance = Mathf.Abs(deltaMove.x);
                            float descendDeltaMoveY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            deltaMove.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(deltaMove.x);
                            deltaMove.y -= descendDeltaMoveY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 deltaMove) {
        if(hit) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle > maxSlopeAngle) {
                deltaMove.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(deltaMove.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    void ResetFallingThroughPlatform() {
        collisions.fallingThroughPlatform = false;
    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public int faceDir;
        public bool fallingThroughPlatform;

        public void Reset() {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
