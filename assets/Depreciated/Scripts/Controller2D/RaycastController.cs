using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour {
    public LayerMask collisionMask;

    public const float skinWidth = .015f;
    const float dstBetweenRays = .25f;
    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public int verticalRayCount;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D boxCollider;
    public RaycastOrigins raycastOrigins;

    public virtual void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start() {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins() {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        //Calculate world space locations for corners
        raycastOrigins.bottomLeft = transform.TransformPoint(new Vector2(boxCollider.size.x, boxCollider.size.y) * 0.5f + boxCollider.offset);
        raycastOrigins.bottomRight = transform.TransformPoint(new Vector2(-boxCollider.size.x, boxCollider.size.y) * 0.5f + boxCollider.offset);
        raycastOrigins.topLeft = transform.TransformPoint(new Vector2(boxCollider.size.x, -boxCollider.size.y) * 0.5f + boxCollider.offset);
        raycastOrigins.topRight = transform.TransformPoint(new Vector2(-boxCollider.size.x, -boxCollider.size.y) * 0.5f + boxCollider.offset);
    }

    public void CalculateRaySpacing() {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(raycastOrigins.bottomLeft, 0.1f);
        Gizmos.DrawSphere(raycastOrigins.bottomRight, 0.1f);
        Gizmos.DrawSphere(raycastOrigins.topLeft, 0.1f);
        Gizmos.DrawSphere(raycastOrigins.topRight, 0.1f);
    }

    /*void OnDrawGizmos() {
        BoxCollider2D b = GetComponent<BoxCollider2D>();

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(b.offset + new Vector2(b.size.x, b.size.y) * 0.5f), 0.1f);
        Gizmos.DrawSphere(transform.TransformPoint(b.offset + new Vector2(b.size.x, -b.size.y) * 0.5f), 0.1f);
        Gizmos.DrawSphere(transform.TransformPoint(b.offset + new Vector2(-b.size.x, b.size.y) * 0.5f), 0.1f);
        Gizmos.DrawSphere(transform.TransformPoint(b.offset + new Vector2(-b.size.x, -b.size.y) * 0.5f), 0.1f);

        Gizmos.DrawSphere(transform.TransformPoint(b.offset + new Vector2(b.size.x, b.size.y) * 0.5f), 0.1f);
        Gizmos.DrawSphere(transform.TransformPoint(b.offset + new Vector2(b.size.x, -b.size.y) * 0.5f), 0.1f);
        Gizmos.DrawSphere(transform.TransformPoint(b.offset + new Vector2(-b.size.x, b.size.y) * 0.5f), 0.1f);
        Gizmos.DrawSphere(transform.TransformPoint(b.offset + new Vector2(-b.size.x, -b.size.y) * 0.5f), 0.1f);
    }*/

    public struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
