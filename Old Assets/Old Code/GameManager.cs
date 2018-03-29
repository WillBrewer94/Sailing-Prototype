using UnityEngine;
using System.Collections;

//State Enum
public enum GameState { STARTSCREEN, BATTLESCREEN, SHOPSCREEN, PAUSESCREEN, DEATHSCREEN };

public delegate void OnStateChangeHandler();

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public event OnStateChangeHandler OnStateChange;
    public GameState currentState = GameState.STARTSCREEN;

    //========================================================================
    // StateManager()
    //========================================================================
    // Singleton. Creates new instance of StateManager if one does not exist
    //========================================================================
    public static GameManager Instance {
        get {
            if(GameManager.instance == null) {
                DontDestroyOnLoad(GameManager.instance);
                instance = new GameManager();
            }

            return instance;
        }
    }

    public void OnApplicationQuit() {
        instance = null;
    }

    public void Awake() {
        if(instance != this) {
            Destroy(gameObject);
        }

        instance = this;
    }

    public void SetGameState(GameState state) {
        this.currentState = state;
        OnStateChange();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
