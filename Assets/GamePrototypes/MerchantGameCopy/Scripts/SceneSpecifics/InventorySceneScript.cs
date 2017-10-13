using UnityEngine;
using System.Collections;

public class InventorySceneScript : MonoBehaviour
{

    public InventoryScript A;
    public InventoryScript B;


    // Use this for initialization
    void Start ()
	{
	    FindObjectOfType<InventoryWindowScript>().ShowInventory(A,"A",B,"B");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
