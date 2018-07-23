
using InControl;
using UnityEngine;

public class PlayerActions : PlayerActionSet {
    public PlayerAction Jump;
    public PlayerAction Interact;
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerTwoAxisAction Move;

    public PlayerActions() {
        Jump = CreatePlayerAction("Jump");
        Interact = CreatePlayerAction("Interact");

        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Move Up");
        Down = CreatePlayerAction("Move Down");
        Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
    }
}

