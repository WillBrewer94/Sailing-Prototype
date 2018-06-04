using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[RequireComponent(typeof(Boat))]
public class BoatInput : MonoBehaviour {
    InputDevice device;
    InputControl control;

    Boat player;
    public PlayerActions playerActions;

    void Start() {
        player = GetComponent<Boat>();
        playerActions = CreateWithDefaultBindings();
    }

    void Update() {
        player.SetDirectionalInput(playerActions.Move);
    }

    //=========================
    //        Bindings 
    //=========================
    PlayerActions CreateWithDefaultBindings() {
        PlayerActions playerActions = new PlayerActions();

        playerActions.Up.AddDefaultBinding(Key.UpArrow);
        playerActions.Down.AddDefaultBinding(Key.DownArrow);
        playerActions.Left.AddDefaultBinding(Key.LeftArrow);
        playerActions.Right.AddDefaultBinding(Key.RightArrow);

        playerActions.Up.AddDefaultBinding(Key.W);
        playerActions.Down.AddDefaultBinding(Key.S);
        playerActions.Left.AddDefaultBinding(Key.A);
        playerActions.Right.AddDefaultBinding(Key.D);

        playerActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        playerActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
        playerActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
        playerActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);

        playerActions.ListenOptions.IncludeUnknownControllers = true;
        playerActions.ListenOptions.MaxAllowedBindings = 4;
        playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;

        return playerActions;
    }
}
