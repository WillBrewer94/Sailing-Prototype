using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ParallaxScroll : MonoBehaviour {
	public float speed;

    //Reduce speed by a set percentage per depth;
    public float depthReduction = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float offset = GetComponent<Renderer>().material.mainTextureOffset.x;

        offset += (speed * 0.0005f) * depthReduction;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, 0.0f);
	}
}
