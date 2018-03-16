using UnityEngine;
using System.Collections;

public class ClearFogOfWarScript : MonoBehaviour
{
    public float Radius = 5;
    private FogOfWarScript fogOfWarScript;
    // Use this for initialization
    void Start()
    {
        fogOfWarScript = FindObjectOfType<FogOfWarScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fogOfWarScript != null)
            fogOfWarScript.MakeVisible(transform.position, Radius);
    }
}
