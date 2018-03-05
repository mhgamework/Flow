using UnityEngine;
using System.Collections;
using Assets;
using System;
using UnityEngine.EventSystems;
using Assets.UnityAdditions;
using System.Collections.Generic;
using System.Linq;
using Assets.GamePrototypes.DontStarve;

public class Tree : MonoBehaviour, IInteractable, IPointerEnterHandler, IPointerExitHandler
{


    public float Durability = 3;
    public GameObject Output;




    // Use this for initialization
    void Start()
    {
        InteractablesSingleton.Instance.RegisterInteractable(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

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
        if (!player.IsHoldingTool("axe")) return;
        if (Durability <= 0) return;

        Durability -= Time.deltaTime;

        var step = (Durability % 1) / 1.0f;
        var movement = 0f;
        if (step < 0.5f)
            movement = 0;
        else if (step < 0.75f)
            movement = EasingFunction.EaseInBack(0,1,(step - 0.5f) / 0.25f);
        else
            movement = EasingFunction.EaseOutBack(0,1,1 - ((step - 0.75f) / 0.25f));

        Debug.Log(movement);

        transform.localRotation = Quaternion.AngleAxis(movement * 5, Vector3.forward);


        if (Durability > 0) return;


        StartCoroutine(fall().GetEnumerator());


    }

    private IEnumerable<YieldInstruction> fall()
    {

        Output.transform.SetParent(null, true); // detach so it does not rotate

        var time = 0f;
        while (time < 1f)
        {
            transform.localRotation = Quaternion.AngleAxis(EasingFunction.EaseInCubic(0,1,  time) * 90, Vector3.forward);
            time += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }

        transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);

        InteractablesSingleton.Instance.UnRegisterInteractable(this);


        foreach (var o in Output.transform.GetChildren<Transform>().ToArray()) // To array, since we are removing children!
            o.SetParent(null, true);

        Destroy(Output);


        time = 0f;
        while (time < 3f)
        {
            transform.position += Vector3.down * Time.deltaTime * 0.3f;
            time += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }

        Destroy(gameObject);

    }


    public void OnPointerExit(PointerEventData eventData)
    {
        TextCursorTooltipSingleton.Instance.ClearTooltip();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PlayerScript.Instance.IsHoldingTool("axe"))
            TextCursorTooltipSingleton.Instance.SetTooltip("Chop tree", null);
        else
            TextCursorTooltipSingleton.Instance.SetTooltip("Examine tree", null);
    }
}
