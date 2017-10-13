using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using Assets.DontStarve.Crafting;

public class RecipeListViewItem : MonoBehaviour, IPointerClickHandler {

    public RecipeDetailsView DetailsView;
    public Recipe Recipe;
    public PrefabPreviewImageScript PreviewImage;

    public void OnPointerClick(PointerEventData eventData)
    {
        DetailsView.Show(Recipe);
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    internal void SetRecipe(Recipe recipe)
    {
        Recipe = recipe;
        PreviewImage.Prefab = recipe.ItemModel;
    }
}
