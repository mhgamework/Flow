using UnityEngine;
using System.Collections;
using Assets;
using System;
using Assets.GamePrototypes.DontStarve;

public class Mushroom : MonoBehaviour,IInteractable {

    public float AmountHealthLost = 0.2f;
    public float AmountFoodGained = 0.1f;
    public string ResourceType;
    public float ResourceAmount=1;



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
        player.takeDamage(AmountHealthLost);
        player.gainFood(AmountFoodGained);

        InteractablesSingleton.Instance.UnRegisterInteractable(this);
        Destroy(gameObject);
    }

    // Use this for initialization
    void Start () {
        InteractablesSingleton.Instance.RegisterInteractable(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
