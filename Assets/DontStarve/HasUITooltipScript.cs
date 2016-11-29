using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class HasUITooltipScript : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {

	public string LeftMouseText;
	public string RightMouseText;

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("Enter " + this);
		TextCursorTooltipSingleton.Instance.SetTooltip(LeftMouseText, RightMouseText);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("Exit " + this);
		TextCursorTooltipSingleton.Instance.ClearTooltip();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
