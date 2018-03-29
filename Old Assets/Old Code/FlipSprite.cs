using UnityEngine;
using System.Collections;

public class FlipSprite : MonoBehaviour {
    Vector2 parentVelocity;
    Player parent;
    bool facingRight = true;

	// Use this for initialization
	void Start () {
        parent = GetComponentInParent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(parent.dirInput.x > 0 && !facingRight) {
            Flip();
        } else if(parent.dirInput.x < 0 && facingRight) {
            Flip();
        }
	}

    void Flip() {
        facingRight = !facingRight;
        float theScale = transform.localScale.x;
        theScale *= -1;
        transform.localScale = new Vector2(theScale, transform.localScale.y);
    }
}
