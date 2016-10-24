using UnityEngine;
using System.Collections;
using Assets;

[RequireComponent(typeof(InventoryScript))]
public class InteractableInventoryScript : MonoBehaviour, IPlayerInteractable
{

    public string InventoryName = "Chest";

    [SerializeField]
    private InventoryScript inventory;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact(PlayerMovementScript playerMovementScript)
    {
        playerMovementScript.MoveTo(transform.position, () =>
        {
            var window = FindObjectOfType<InventoryWindowScript>();
            window.ShowInventory(inventory, InventoryName, playerMovementScript.GetComponent<InventoryScript>(),
                "Player");
        });
    }


}
