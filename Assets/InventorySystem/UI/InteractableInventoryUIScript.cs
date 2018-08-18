using UnityEngine;
using System.Collections;
using System;

public class InteractableInventoryUIScript : MonoBehaviour
{

    [SerializeField]
    public InventoryScript TargetInventory;
    [SerializeField]
    public ItemSlotScript ItemSlotPrefab;

    public int NumSlots = 16;
    private Action unregisterListeners = null;

    // Use this for initialization
    void Start()
    {
        if (TargetInventory != null)
            SetInventory(TargetInventory);
        BuildUI();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuildUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        transform.DetachChildren();
        for (int i = 0; i < NumSlots; i++)
        {
            var slot = Instantiate(ItemSlotPrefab,transform);
            slot.transform.parent = transform;
            slot.inventoryUI = this;
            slot.SlotNum = i;
        }
    }
    public void UpdateUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.GetComponent<ItemSlotScript>().UpdateUI();
        }
    }

    public ItemSlotScript GetSlot(int num)
    {
        return transform.GetChild(num).GetComponent<ItemSlotScript>();
    }


    public void SetInventory(InventoryScript inventoryScript)
    {
        if (unregisterListeners != null)
        {
            unregisterListeners();
            unregisterListeners = null;
        }
        TargetInventory = inventoryScript;

        BuildUI();
        UpdateUI();

        unregisterListeners = TargetInventory.RegisterChangeListener(iSlot => transform.GetChild(iSlot).GetComponent<ItemSlotScript>().UpdateUI());

    }
}
