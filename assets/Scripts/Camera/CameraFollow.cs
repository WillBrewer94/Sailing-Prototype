using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public Controller2D target;
    public float verticalOffset;
    public Vector2 focusAreaSize;

    public float lookAheadDistX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;

    FocusArea focusArea;

    float currLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;

    void Start() {
        focusArea = new FocusArea(target.GetComponent<Collider2D>().bounds, focusAreaSize);
    }

    void LateUpdate() {
        focusArea.Update(target.GetComponent<Collider2D>().bounds);

        Vector2 focusPos = focusArea.center + Vector2.up * verticalOffset;

        if(focusArea.velocity.x != 0) {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            if(Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0) {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDistX;

            } else {
                if(!lookAheadStopped) {
                    lookAheadStopped = true;
                    targetLookAheadX = currLookAheadX + (lookAheadDirX * lookAheadDistX - currLookAheadX) / 4f;
                }
            }
        }

        currLookAheadX = Mathf.SmoothDamp(currLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
        
        focusPos.y = Mathf.SmoothDamp(transform.position.y, focusPos.y, ref smoothVelocityY, verticalSmoothTime);
        focusPos += Vector2.right * currLookAheadX;
        transform.position = (Vector3) focusPos + Vector3.forward * -10;
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    struct FocusArea {
        public Vector2 center;
        public Vector2 velocity;
        public float left, right;
        public float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size) {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            top = targetBounds.min.y + size.y;
            bottom = targetBounds.min.y;

            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds) {
            float shiftX = 0;

            if(targetBounds.min.x < left) {
                shiftX = targetBounds.min.x - left;
            } else if(targetBounds.max.x > right) {
                shiftX = targetBounds.max.x - right;
            }

            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if(targetBounds.min.y < bottom) {
                shiftY = targetBounds.min.y - bottom;
            } else if(targetBounds.max.y > top) {
                shiftY = targetBounds.max.y - top;
            }

            top += shiftY;
            bottom += shiftY;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}
