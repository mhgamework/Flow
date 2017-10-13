using UnityEngine;
using System.Collections;
using Assets;
using System;

public class Harvestable : MonoBehaviour, IInteractable
{

    public String ResourceType;
    public int Amount;

    public bool GrowsBack = false;
    public float GrowBackTime = 5;
    public float GrowBackNext = 0;
    public bool Grown = true;

    public Transform HarvestedModel;
    public Transform UnHarvestedModel;

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
        if (!Grown) return;
        if (!player.TryAddResource(ResourceType, Amount)) return;

        if (!GrowsBack)
        {
            Destroy(gameObject);
            
        }
        else
        {
            Grown = false;
            updateModel();
            GrowBackNext = GrowBackTime;
        }

        InteractablesSingleton.Instance.UnRegisterInteractable(this);

    }

    private void updateModel()
    {
        HarvestedModel.gameObject.SetActive(!Grown);
        UnHarvestedModel.gameObject.SetActive(Grown);

    }




    // Use this for initialization
    void Start()
    {
        InteractablesSingleton.Instance.RegisterInteractable(this);
        updateModel();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Grown)
        {
            GrowBackNext -= Time.deltaTime;
            if (GrowBackNext < 0)
            {
                Grown = true;
                updateModel();
                InteractablesSingleton.Instance.RegisterInteractable(this);
            }
        }

    }
}
