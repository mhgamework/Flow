using UnityEngine;
using System.Collections;
using Assets;
using System;

public class Harvestable : MonoBehaviour,IInteractable {

    public String ResourceType;
    public int Amount;

    public float getDistance(PlayerScript player)
    {
        return (player.transform.position - transform.position).magnitude;
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public void Interact(PlayerScript player)
    {
        if (!player.TryAddResource(ResourceType, Amount)) return;

        Destroy(gameObject);
        InteractablesSingleton.Instance.UnRegisterInteractable(this);
    }




    // Use this for initialization
    void Start () {
        InteractablesSingleton.Instance.RegisterInteractable(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
