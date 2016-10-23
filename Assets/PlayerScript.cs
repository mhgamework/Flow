using UnityEngine;
using System.Collections;
using System;

public class PlayerScript : MonoBehaviour {

    public float MoveSpeed=1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var t = Time.deltaTime* MoveSpeed;

        var dir = new Vector3();
        if (Input.GetKey(KeyCode.Z))
            dir += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.S))
            dir += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.Q))
            dir += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.D))
            dir += new Vector3(1, 0, 0);
        move(dir.normalized * t);
    }

    private void move(Vector3 delta)
    {
        transform.position += delta;
    }
}
