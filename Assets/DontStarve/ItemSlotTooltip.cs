using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

/// <summary>
/// Implements the mouse hover tooltip for ui item stacks
/// </summary>
public class ItemSlotTooltip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
    ItemSlotScript itemSlotScript;

    // Use this for initialization
    void Start () {
        itemSlotScript = GetComponent<ItemSlotScript>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        var inventoryItem = itemSlotScript.GetSlotStack();
        if (inventoryItem == null) return;
        var text = inventoryItem.GetItemType().Name;
        TextCursorTooltipSingleton.Instance.SetTooltip(text, null);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TextCursorTooltipSingleton.Instance.ClearTooltip();
    }

}
