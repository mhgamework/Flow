using System;
using UnityEngine;
using System.Collections;

public class HightlightScript : MonoBehaviour {
    public Color HighlightColor;
    private Color originalColor;
    private bool highlighted = false;


    // Use this for initialization
	void Start ()
	{
        if (GetComponent<MeshRenderer>() == null) throw new InvalidOperationException("Only works when there is a renderer");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Highlight()
    {
        if (highlighted) return;
        originalColor = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.color = HighlightColor;
        highlighted = true;
    }

    public void Reset()
    {
        highlighted = false;
        GetComponent<MeshRenderer>().material.color = originalColor;
    }

}
