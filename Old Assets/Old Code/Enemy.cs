using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    public float radius = 5;

    public List<Action> moves = new List<Action>();

    public GameObject areaAttack;
    public GameObject lineAttack;
    public GameObject currAttack;
    public LineRenderer attackPath;

    //LineAttack variables
    public Vector2 target;

    public bool isMoving = false;
    public bool isCollide = false;
    public int health = 2;

	// Use this for initialization
	void Start () {
        attackPath = GetComponent<LineRenderer>();
        moves.Add(() => AreaAttack());
        moves.Add(() => LineAttack());
	}

    public void OnPause() {
        //Choose and execute move
        attackPath.enabled = false;
        isMoving = false;

        moves[UnityEngine.Random.Range(0, moves.Count)].Invoke();
    }

    public GameObject GetAttack() {
        return currAttack;
    }

    public void ToggleLineRenderer() {
        attackPath.enabled = !attackPath.enabled;
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Slam") {
            isCollide = true;
            Debug.Log("Enter");
        }
    }

    public void OnTriggerExit2D(Collider2D collision) {
        isCollide = false;
        Debug.Log("Exit");
    }

    public IEnumerator Slammed(Vector2 targetPos, Vector2 startPos) {
        float step = (10 / (targetPos - startPos).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while(t <= 1.0f) {
            t += step; // Goes from 0 to 1, incrementing by step each time
            transform.position = Vector3.Lerp(targetPos, startPos, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }

        transform.position = startPos;
    }

    public void SetHealth(int health) {
        this.health = health;

        if(health < 1) {
            Destroy(gameObject);
        }
    }

    public int GetHealth() {
        return health;
    }

    //====================================================
    //                  Enemy Methods                    =
    //====================================================

    //Fires an area of effect attack of radius r
    public void AreaAttack() {
        currAttack = Instantiate(areaAttack, transform.position, Quaternion.identity);
    }

    //Chooses a direction close to the player and fires a beam
    public void LineAttack() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        attackPath.enabled = true;
        attackPath.SetPosition(0, transform.position);
        attackPath.SetPosition(1, player.transform.position);
        isMoving = true;
    }
}
