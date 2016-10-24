using UnityEngine;
using System.Collections;

public class InteractableInventoryUIScript : MonoBehaviour
{

    [SerializeField]
    public InventoryScript TargetInventory;
    [SerializeField]
    public ItemSlotScript ItemSlotPrefab;

    public int NumSlots = 16;

    // Use this for initialization
    void Start()
    {
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
            var slot = Instantiate(ItemSlotPrefab);
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


    public void SetInventory(InventoryScript inventoryScript)
    {
        TargetInventory = inventoryScript;
        BuildUI();
        UpdateUI();
    }
}
