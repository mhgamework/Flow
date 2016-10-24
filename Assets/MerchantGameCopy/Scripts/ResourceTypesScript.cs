using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;

public class ResourceTypesScript : MonoBehaviour
{
    [SerializeField]
    private List<ResourceType> Types;

    public static ResourceTypesScript Instance()
    {
        var ret = FindObjectOfType<ResourceTypesScript>();
        if (!ret) Debug.LogError("No instance of RenderToTextureScript found in scene!");
        return ret;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public ResourceType Find(string identifier)
    {
        return Types.FirstOrDefault(t => t.Identifier == identifier);
    }
}
