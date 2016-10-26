using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using Assets;
using UnityEngine.EventSystems;

public class PlayerScript : Singleton<PlayerScript>, IPointerClickHandler, IPointerEnterHandler,IPointerExitHandler
{

    public float MoveSpeed = 1;

    public float Food = 1;
    public float Health = 1;
    public float Light = 1;

    public float FoodDecrease = 0.1f;
    public float MaxInteractDistance = 2;
    public float MinInteractDistance = 0.5f;

    public InventoryScript HotbarInventory;

    // Use this for initialization
    void Start()
    {

    }

    public bool TryAddResource(string resourceType, int amount)
    {
        //TODO: limiting
        HotbarInventory.AddResources(resourceType, amount);
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        var t = Time.deltaTime * MoveSpeed;

        var dir = new Vector3();
        if (Input.GetKey(KeyCode.Z))
            dir += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.S))
            dir += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.Q))
            dir += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.D))
            dir += new Vector3(1, 0, 0);
        move(dir.normalized * t);


        Food -= FoodDecrease * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
            tryInteract();
    }

    public void gainFood(float amountFoodGained)
    {
        Food = Mathf.Clamp(Food + amountFoodGained, 0, 1);
    }

    public void takeDamage(float amountHealthLost)
    {
        Health = Mathf.Clamp(Health - amountHealthLost, 0, 1);
        if (Health == 0)
            Debug.Log("YOU DIEEED!");
    }

    private void tryInteract()
    {
        var interactables = InteractablesSingleton.Instance.GetAllInteractables();
        var closest = interactables.Where(f => f.getDistance(this) < MaxInteractDistance).OrderBy(f => f.getDistance(this)).FirstOrDefault();

        if (closest == null) return;

        var dist = closest.getDistance(this);
        if (dist <= MinInteractDistance)
        {
            closest.interact(this);
        }
        else
        {
            // Walk to
            var diff = closest.getPosition() - transform.position;
            dist = diff.magnitude;

            var distanceToStep = MoveSpeed * Time.deltaTime;
            distanceToStep = Mathf.Min(diff.magnitude, distanceToStep);

            transform.position += distanceToStep * diff.normalized;
        }

    }

    private void move(Vector3 delta)
    {
        transform.position += delta;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!DraggableItemStackScript.Instance().IsDragging) return;
        var itemType = DraggableItemStackScript.Instance().stack.GetItemType();
        if (itemType == null) return;

        if (itemType.IsEdible)
        {
            itemType.Eat(this);
            DraggableItemStackScript.Instance().RemoveItems(1);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!DraggableItemStackScript.Instance().IsDragging) return;
        var itemType = DraggableItemStackScript.Instance().stack.GetItemType();
        if (itemType == null) return;

        if (itemType.IsEdible)
        {
            TextCursorTooltipSingleton.Instance.SetTooltip("Eat",null);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TextCursorTooltipSingleton.Instance.ClearTooltip();
    }
}
