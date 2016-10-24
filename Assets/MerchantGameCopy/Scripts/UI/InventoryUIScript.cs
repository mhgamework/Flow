using UnityEngine;
using System.Collections;

public class InventoryUIScript : MonoBehaviour
{

    public InventoryScript Player;


	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var i = 0;
	    foreach(var s in GetComponentsInChildren<InventoryUIStack>(true))
	    {
	        if (Player.Inventory.Count <= i)
	        {
	            s.gameObject.SetActive(false);
	            continue;
	        }
            s.gameObject.SetActive(true);
	        s.UpdateUI(Player.Inventory[i].ResourceType, Player.Inventory[i].Amount);
            i++;
	    }
	}
}
