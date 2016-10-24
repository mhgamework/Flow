using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
[ExecuteInEditMode]
public class PrefabPreviewImageScript : MonoBehaviour
{

    public GameObject Prefab;

    private GameObject lastPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (lastPrefab == Prefab) return;
	    GetComponent<RawImage>().texture = RenderToTextureScript.Instance().PrefabToTexture(Prefab);
	    lastPrefab = Prefab;
	}
}
