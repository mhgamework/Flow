using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets.DontStarve.Crafting;

public class RecipeCategoryListItemView : MonoBehaviour, IPointerClickHandler {

    public List<Recipe> RecepiList = new List<Recipe>();
    public int test;

	public bool Selected { get; private set; }
    private List<Action> onClickListeners = new List<Action>();

    public void OnPointerClick(PointerEventData eventData)
	{
        onClickListeners.ForEach(a => a());
	}

	public void RegisterOnClick(Action p)
	{
        onClickListeners.Add(p);
	}

	// Use this for initialization
	void Start () {
	
	}

    public List<Recipe> GetRecipeList()
    {
        return RecepiList;
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void SetSelected(bool selected)
    {
        Selected = selected;
        // TODO: highlighting!
    }
}
