using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Assets.DontStarve.Inventory;

public class DraggableItemStackScript : MonoBehaviour
{

    public bool IsDragging { get; private set; }

    public InventoryScript.InventoryItem stack { get; private set; }
    public InventoryUIStack uiStack;

    public static DraggableItemStackScript Instance()
    {
        return FindObjectOfType<DraggableItemStackScript>();
    }

    public ItemType GetDraggingItemType()
    {
        if (!IsDragging) return null;
        return stack.GetItemType();
    }

    // Use this for initialization
    void Start()
    {
        stack = InventoryScript.InventoryItem.Empty;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDragging)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void StartDrag(InventoryScript.InventoryItem stack)
    {
        if (IsDragging) throw new InvalidOperationException();
        IsDragging = true;
        this.stack = stack;
        UpdateUI();


    }

    public void StopDrag()
    {
        if (!IsDragging) throw new InvalidOperationException();
        IsDragging = false;
        UpdateUI();
    }


    public void UpdateUI()
    {
        uiStack.gameObject.SetActive(IsDragging);
        if (IsDragging)
            uiStack.UpdateUI(stack);
    }

    public void RemoveItems(int amount)
    {
        if (stack.Amount < amount) throw new Exception("Not enough items!");
        stack.Amount -= amount;

        if (stack.Amount == 0)
            StopDrag();
        else
            UpdateUI();
    }
}
