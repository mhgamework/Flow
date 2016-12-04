using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using Assets.UnityAdditions;

public class FirePitScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public float Fuel = 0;
    public float MaxFuel = 4;
    public float BurnRate = 1;

    public bool isStonePit = true;
    public Transform StonesTransform;

    public Transform FuelStates;

    private int visibleState;

    // Use this for initialization
    void Start()
    {
        visibleState = 0;
        foreach (var f in FuelStates.transform.GetChildren<Transform>())
        {
            f.gameObject.SetActive(false);
        }

        FuelStates.GetChild(0).gameObject.SetActive(true);
        visibleState = 0;
    }

    // Update is called once per frame
    void Update()
    {
        StonesTransform.gameObject.SetActive(isStonePit);
        if (Fuel == 0 && !isStonePit)
        {
            TextCursorTooltipSingleton.Instance.ClearTooltip();
            Destroy(gameObject);
            return;
        }

        Fuel = Mathf.Max(0, Fuel - Time.deltaTime * BurnRate);

        if (Fuel == 0)
            setFuelState(0);
        else
        {
            // 0..1  1..2 2..3 => 1 2 3

            var targetState = (int)((Fuel / MaxFuel) * (FuelStates.childCount - 1));

            setFuelState(Math.Min(targetState + 1, FuelStates.childCount - 1));

        }

    }

    private void setFuelState(int v)
    {
        if (visibleState == v) return;
        FuelStates.GetChild(visibleState).gameObject.SetActive(false);

        visibleState = v;

        FuelStates.GetChild(visibleState).gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var type = DraggableItemStackScript.Instance().GetDraggingItemType();
        if (type == null) return;

        if (type.IsFuel)
        {
            Fuel = Mathf.Min(Fuel + type.FuelAmount, MaxFuel);
            DraggableItemStackScript.Instance().RemoveItems(1);
            return;
        }

        if (!isBurning()) return;
        if (type.IsCookable)
        {
            string newType = type.CookedItemType;

            if (PlayerScript.Instance.TryAddResource(newType, 1))
                DraggableItemStackScript.Instance().RemoveItems(1);
            return;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHoldingFuel())
            TextCursorTooltipSingleton.Instance.SetTooltip("Add Fuel", null);
        else if (!isBurning())
            TextCursorTooltipSingleton.Instance.SetTooltip("Examine firepit", null);
        else if (isHoldingCookable())
            TextCursorTooltipSingleton.Instance.SetTooltip("Cook", null);
        else
            TextCursorTooltipSingleton.Instance.SetTooltip("Examine firepit", null);



    }

    private bool isBurning()
    {
        return Fuel > 0;
    }

    private bool isHoldingFuel()
    {
        if (!DraggableItemStackScript.Instance().IsDragging) return false;
        if (DraggableItemStackScript.Instance().GetDraggingItemType().IsFuel) return true;
        return false;
    }

    private bool isHoldingCookable()
    {
        if (!DraggableItemStackScript.Instance().IsDragging) return false;
        if (DraggableItemStackScript.Instance().GetDraggingItemType().IsCookable) return true;
        return false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TextCursorTooltipSingleton.Instance.ClearTooltip();
    }




}
