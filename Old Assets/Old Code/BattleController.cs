using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

//Controller for battle scenes
public class BattleController : MonoBehaviour {
    //Singleton battle manager
    private static BattleController shared;

    //Configuration Values
    public float cursorSpeed = 10;

    //Battle UI
    public Text turnText;
    public Text playerHealth;
    public Text enemyHealth;
    public Text currAttack;
    public GameObject blinkCursor;
    public GameObject areaAttack;
    public GameObject lineAttack;

    //This is messy plz dont look
    public GameObject slamAreaPrefab;
    public GameObject slamArea;

    //Characters
    public GameObject player;
    public GameObject enemy;

    //Delta time values for gameobjects
    public float playerDeltaTarget = 0;
    public float playerDelta = 0;
    public float enemyDelta = 0;
    public float enemyDeltaTarget = 0;
    public float pauseSmoothTime = 0.5f;
    public float timer;
    public float turnTime;

    //Is Time Paused
    public bool isPause = false;

    public bool isBattle = false;

    void Awake() {
        //Make sure only one battle manager exists at a time
        if(shared == null) {
            shared = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        timer = turnTime;
        player.GetComponent<Player>().ToggleGravity();
        ActiveTurn();
    }

    void Update() {
        if(isBattle) {
            if(isPause) {
                //Stop animations and player movement
                player.GetComponent<Animator>().speed = 0;
                Mathf.SmoothDamp(playerDelta, playerDeltaTarget, ref playerDelta, pauseSmoothTime);

                //Show or Hide Attack UI
                string currAttack = player.GetComponent<Player>().GetCurrAttack();

                if(currAttack.Equals("Blink")) {
                    //Move Blink Cursor
                    blinkCursor.SetActive(true);
                    blinkCursor.transform.Translate(player.GetComponent<Player>().dirInput * Time.deltaTime * cursorSpeed);

                    if(slamArea) {
                        Destroy(slamArea);
                        GameObject[] enemyList = GetEnemyList();

                        foreach(GameObject enemy in enemyList) {
                            enemy.GetComponent<Enemy>().isCollide = false;
                        }
                    }

                } else if(currAttack.Equals("Slam")) {
                    blinkCursor.SetActive(false);

                    //This is the messiest method in the history of methods, maybe ever.
                    //Change it dumbass.
                    if(!slamArea) {
                        Vector2 playerPos = player.transform.position;
                        playerPos.x += 0.3f;
                        playerPos.y -= 0.4f;

                        slamArea = Instantiate(slamAreaPrefab, playerPos, Quaternion.identity);
                    }
                }

            } else {
                //Start animations and player movement
                playerDelta = Time.deltaTime;
                player.GetComponent<Animator>().speed = 1;

                //Execute active turn code
                ActiveTurn();

                //Hide Line and Cursor
                blinkCursor.SetActive(false);
            }
        }
    }

    //Singleton Access Method
    public static BattleController Shared() {
        return shared;
    }

    //Getter method for isPaused
    public bool IsPaused() {
        return isPause;
    }

    //Switches pause state and runs relevant code
    public void SwitchPause() {
        isPause = !isPause;
    }

    //====================================================
    //                  Helper Methods                   =
    //====================================================

    //Countdown timer for x seconds, stored in the timer variable
    public void ActiveTurn() {
        if(timer > 0) {
            //THIS CODE ACTIVATES DURING ACTIVE TURN

            timer -= Time.deltaTime;

            GameObject[] enemies = GetEnemyList();

            //Move enemy if lineAttack chosen
            float step = 30 * Time.deltaTime;

            foreach(GameObject enemy in enemies) {
                if(enemy.GetComponent<Enemy>().isMoving) {
                    enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.GetComponent<Enemy>().attackPath.GetPosition(1), step);
                }
            }

        } else {
            isPause = true;
            timer = turnTime;

            //THIS CODE ACTIVATES AT END OF ACTIVE TURN

            Player p = player.GetComponent<Player>();

            //Run logic for hurting player
            if(player.GetComponent<Player>().isCollide) {
                player.GetComponent<Player>().SetHealth(p.GetComponent<Player>().GetHealth() - 1);
            }

            //Remove attack objects from previous turn, stored as current attack in each enemy
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Enemy");

            foreach(GameObject gameObject in gameObjects) {
                Destroy(gameObject.GetComponent<Enemy>().GetAttack());
            }

            //Run OnPause logic for enemies
            GameObject[] enemies = GetEnemyList();

            foreach(GameObject enemy in enemies) {
                enemy.GetComponent<Enemy>().OnPause();
            }
        }
    }

    //Finds all the active enemies on the scene
    public GameObject[] GetEnemyList() {
        return GameObject.FindGameObjectsWithTag("Enemy");
    }
}
