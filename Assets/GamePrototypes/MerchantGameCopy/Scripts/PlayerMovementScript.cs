using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InventoryScript))]
[RequireComponent(typeof(HeroScript))]
public class PlayerMovementScript : MonoBehaviour
{


    public Vector3 TargetedPosition;
    [SerializeField]
    private ClickedPositionIndicatorScript ClickedPositionIndicator;

    [SerializeField]
    private Transform PlayerTransform;

    public Vector3 TargetMovePosition;
    private Action onArriveAction;

    private HeroScript hero;

    // Use this for initialization
    void Start()
    {
        TargetMovePosition = transform.position;
        hero = GetComponent<HeroScript>();
    }

    // Update is called once per frame
    void Update()
    {
        var dir = TargetMovePosition - PlayerTransform.position;

        var f = Time.deltaTime * hero.ActiveMoveSpeed;
        if (dir.magnitude < f)
        {
            PlayerTransform.position = TargetMovePosition;
            if (onArriveAction != null) onArriveAction();
            onArriveAction = null;
        }
        else PlayerTransform.position = PlayerTransform.position + dir.normalized * f;


        var hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition)).OrderBy(h => h.distance);
        if (!hits.Any()) return;

        // Filter hits

        TargetedPosition = hits.First().point;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;
            var interactables = hits.First().collider.GetComponentsInParent(typeof(IPlayerInteractable)).Cast<IPlayerInteractable>().ToArray();
            if (!interactables.Any())
            {
                // Move towards
                MoveTo(TargetedPosition);
                return;
            }
            if (interactables.Count() > 1) throw new InvalidOperationException("Unexpected behavioru with nested interactbales, not supported");

            interactables.First().Interact(this);

        }

        /*foreach (var hit in hits)
	    {
	        
	    }*/


    }




    public void MoveTo(Vector3 position)
    {
        MoveTo(position, null);
    }
    public void MoveTo(Vector3 position, Action onArrive)
    {
        TargetMovePosition = position;
        ClickedPositionIndicator.Show(position);
        onArriveAction = onArrive;
    }



    public void PickupResources(string resourceType, int amount)
    {
        GetComponent<InventoryScript>().AddResources(resourceType, amount);
    }


    public Vector3 GetPosition()
    {
        return PlayerTransform.position;
    }
}
