using UnityEngine;
using System.Collections;
using Assets;

public class HighlightOnMouse : MonoBehaviour {

    private Color HighlightColor = Color.blue;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnMouseExit()
    {
        foreach (var r in GetComponentsInChildren<MeshRenderer>())
        {
            var h = r.GetOrCreateComponent<HightlightScript>();
            h.Reset();
        }
    }

    public void OnMouseEnter()
    {
        foreach (var r in GetComponentsInChildren<MeshRenderer>())
        {
            var h = r.GetOrCreateComponent<HightlightScript>();
            h.HighlightColor = HighlightColor;
            h.Highlight();
        }
    }
}
