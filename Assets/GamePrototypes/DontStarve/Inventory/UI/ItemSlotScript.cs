﻿using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Assets.DontStarve.Inventory;
using UnityEngine.UI;

public class ItemSlotScript : MonoBehaviour, IPointerClickHandler
{

    public InteractableInventoryUIScript inventoryUI;
    public int SlotNum;
    public InventoryUIStack uiStack;

    private Color startBackgroundColor;
    // Use this for initialization
    void Start()
    {
        startBackgroundColor = GetComponent<Image>().color;
    }


    // Update is called once per frame
    void Update()
    {

    }

    private InventoryScript GetInventory()
    {
        if (inventoryUI == null) return null;
        return inventoryUI.TargetInventory;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        var dragger = DraggableItemStackScript.Instance();
        var slotStack = GetSlotStack();
        if (!dragger.IsDragging)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                GetInventory().ClearSlot(SlotNum);
                dragger.StartDrag(slotStack);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                var newStack = new InventoryScript.InventoryItem(slotStack.ResourceType, Mathf.CeilToInt(slotStack.Amount / 2f));
                slotStack.Amount -= newStack.Amount;
                GetInventory().SetSlot(SlotNum, slotStack);
                dragger.StartDrag(newStack);

            }

        }
        else
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (dragger.stack.ResourceType == slotStack.ResourceType)
                {
                    // Add it
                    slotStack.Amount += dragger.stack.Amount;
                    GetInventory().SetSlot(SlotNum, slotStack);
                    dragger.StopDrag();
                }
                else
                {
                    // Swap it
                    var swapStack = slotStack;
                    GetInventory().SetSlot(SlotNum, dragger.stack);
                    dragger.StopDrag();
                    if (!swapStack.IsEmpty)
                        dragger.StartDrag(swapStack);
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (dragger.stack.ResourceType == slotStack.ResourceType)
                {
                    // Add it
                    slotStack.Amount++;
                    GetInventory().SetSlot(SlotNum, slotStack);

                    dragger.stack.Amount--;

                }
                if (slotStack.IsEmpty)
                {
                    // Add to empty
                    slotStack.ResourceType = dragger.stack.ResourceType;
                    slotStack.Amount = 1;
                    GetInventory().SetSlot(SlotNum, slotStack);

                    dragger.stack.Amount--;
                }
                if (dragger.stack.Amount <= 0)
                {
                    dragger.StopDrag();
                }
            }
        }

        UpdateUI();
        dragger.UpdateUI();

        /* if (DraggableItemStackScript.DraggingStack == null) return; // do nothing when not dragging

         if (eventData.button == PointerEventData.InputButton.Left)
         {
             var dragStack = DraggableItemStackScript.DraggingStack;
             DraggableItemStackScript.StopDragging();

             if (Contents != null)
             {
                 DraggableItemStackScript.StartDragging(Contents);
             }
             SetStack(dragStack);

         }*/
    }

    public InventoryScript.InventoryItem GetSlotStack()
    {
        if (GetInventory() == null) return null;
        return GetInventory().GetSlot(SlotNum);
    }

    public void UpdateUI()
    {
        var s = GetInventory().GetSlot(SlotNum);
        uiStack.UpdateUI(s);
    }

    public void SetBackgroundColor(Color color)
    {
        GetComponent<Image>().color = color;
    }

    public void ResetBackgroundColor()
    {
        GetComponent<Image>().color = startBackgroundColor;

    }
}
