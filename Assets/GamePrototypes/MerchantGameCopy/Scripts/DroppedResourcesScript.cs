using UnityEngine;
using System.Collections;
using Assets;

[ExecuteInEditMode]
public class DroppedResourcesScript : MonoBehaviour, IPlayerInteractable
{
    public string ResourceType = "gold";
    public int Amount = 1;

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
            playerMovementScript.PickupResources(ResourceType, Amount);
            Destroy(gameObject);

        });
    }


}
