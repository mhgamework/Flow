using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitApplicationScript : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    GetComponent<Button>().onClick.AddListener(() => { Application.Quit(); });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
