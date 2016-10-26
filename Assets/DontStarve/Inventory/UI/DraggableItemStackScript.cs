using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DraggableItemStackScript : MonoBehaviour
{

    public bool IsDragging { get; private set; }

    public InventoryScript.InventoryItem stack { get; private set; }
    public InventoryUIStack uiStack;

    public static DraggableItemStackScript Instance()
    {
        return FindObjectOfType<DraggableItemStackScript>();
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
}
