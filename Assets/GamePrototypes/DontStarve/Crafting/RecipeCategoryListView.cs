using UnityEngine;
using System.Collections;
using MHGameWork.TheWizards.DualContouring;
using System;

public class RecipeCategoryListView : MonoBehaviour {

	public RecipeCategoryListItemView Selected;
    public RecipeListView ListUi;

	// Use this for initialization
	void Start () {

		GetComponentsInChildren<RecipeCategoryListItemView>().ForEach(c =>
		{
			c.RegisterOnClick(() => Select(c));
		});

	}

	private void Select(RecipeCategoryListItemView c)
	{
        if (Selected == c)
            Selected = null;
        else
            Selected = c;

        if (Selected == null)
        {
            ListUi.Hide();
        }
        else
        {
            ListUi.SetRecipeList(c.GetRecipeList());
            ListUi.Show();
        }
	}

	// Update is called once per frame
	void Update () {
	
	}
}
