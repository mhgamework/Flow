using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class FirePitScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isHoldingCookable())
        {
            string newType = null;
            if (DraggableItemStackScript.Instance().stack.ResourceType == "redmushroom")
                newType = "cookedredmushroom";

            if (PlayerScript.Instance.TryAddResource(newType,1))
            {
                DraggableItemStackScript.Instance().stack.Amount--;
                if (DraggableItemStackScript.Instance().stack.Amount == 0)
                    DraggableItemStackScript.Instance().StopDrag();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (isHoldingCookable())
            TextCursorTooltipSingleton.Instance.SetTooltip("Cook", null);
        else
            TextCursorTooltipSingleton.Instance.SetTooltip("Examine firepit", null);



    }

    private bool isHoldingCookable()
    {
        if (!DraggableItemStackScript.Instance().IsDragging) return false;
        if (DraggableItemStackScript.Instance().stack.ResourceType == "redmushroom") return true;
        return false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TextCursorTooltipSingleton.Instance.ClearTooltip();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
