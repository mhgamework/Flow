using UnityEngine;
using System.Collections;
using Assets;

public class MagicAreaScript : MonoBehaviour
{

    public string MagicType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{

	    GetComponentInChildren<MeshRenderer>().material.color = MagicService.Get.GetColor(MagicType);
	}
}
