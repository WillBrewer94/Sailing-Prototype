﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransition : MonoBehaviour {
    public string transitionScene = "TestRoom_Boat";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /*private void OnTriggerStay2D(Collider2D collision) {
        
    }*/

    public string GetRoomTransition() {
        return transitionScene;
    }
}
