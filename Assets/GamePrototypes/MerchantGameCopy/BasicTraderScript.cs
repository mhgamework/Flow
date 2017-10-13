using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicTraderScript : MonoBehaviour
{

    public InventoryScript InputInventory;
    public InventoryScript OutputInventory;
    public List<InventoryScript.InventoryItem> InputItems;
    public List<InventoryScript.InventoryItem> OutputItems;
    public float TimeBetweenTrades = 1;

    // Use this for initialization
    void Start () {
	
	}

    private float lastTrade = float.MinValue;

	// Update is called once per frame
	void Update () {
	    if (Time.timeSinceLevelLoad > lastTrade + TimeBetweenTrades)
	    {
	        Trade();
	    }
	}

    private void Trade()
    {
        foreach (var el in InputItems)
        {
            if (InputInventory.Count(el.ResourceType) < el.Amount) return; // can't trade
        }
        foreach (var el in InputItems)
            InputInventory.RemoveResourcse(el.ResourceType, el.Amount);

        foreach (var el in OutputItems)
            OutputInventory.AddResources(el.ResourceType, el.Amount);

        lastTrade = Time.timeSinceLevelLoad;
    }
}
