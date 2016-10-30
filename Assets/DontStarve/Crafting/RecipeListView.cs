using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.DontStarve.Crafting;
using System.Linq;
using Assets.UnityAdditions;

public class RecipeListView : MonoBehaviour {

	public RecipeDetailsView DetailsView;
    public RecipeListViewItem ItemTemplate;


	// Use this for initialization
	void Start () {
		Hide();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Show()
	{
		gameObject.SetActive(true);
		
	}

	public void Hide()
	{
        DetailsView.Hide();
        gameObject.SetActive(false);
		
	}

	public void SetRecipeList(List<Recipe> p)
	{
        DetailsView.Hide();
        foreach (var c in transform.GetChildren<MonoBehaviour>().ToArray())
        {
            Destroy(c.gameObject);
        }

        foreach(var recipe in p)
        {
            var view = Instantiate(ItemTemplate);
            view.SetRecipe(recipe);
            view.transform.SetParent(transform);
            view.DetailsView = DetailsView;
        }

	}
}
