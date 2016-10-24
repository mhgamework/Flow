using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Deselects all when clicked
/// </summary>
public class RTSDeselector : MonoBehaviour,IPointerClickHandler {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        RTSSelectionService.Instance().DeselectAll();
    }
}
