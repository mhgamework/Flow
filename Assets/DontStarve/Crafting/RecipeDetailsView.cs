using UnityEngine;
using System.Collections;
using Assets.DontStarve.Crafting;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.UnityAdditions;
using MHGameWork.TheWizards.DualContouring;

public class RecipeDetailsView : MonoBehaviour
{

    public Text TitleText;
    public Text DescriptionText;
    public ItemView ItemTemplate;
    public Transform ItemsPanel;
    public Button CraftButton;

    private Recipe activeRecipe;

    // Use this for initialization
    void Start()
    {
        Hide();
        CraftButton.onClick.AddListener(() => Craft());
    }


    // Update is called once per frame
    void Update()
    {
        CraftButton.interactable = CanCraft();
        updateItemCounts();
    }

    private void Craft()
    {
        PlayerScript.Instance.Craft(activeRecipe);
    }

    private bool CanCraft()
    {
        return PlayerScript.Instance.CanCraft(activeRecipe);
    }

    public void Show(Recipe recipe)
    {
        activeRecipe = recipe;
        TitleText.text = recipe.Name;
        DescriptionText.text = recipe.Description;
        createItemsInPanel(recipe.ResourceCost);
        gameObject.SetActive(true);
    }



    public void Hide()
    {
        activeRecipe = null;
        gameObject.SetActive(false);
    }

    private void createItemsInPanel(List<Recipe.ResourceAmount> resourceCost)
    {
        ItemsPanel.GetChildren<ItemView>().ForEach(i => Destroy(i.gameObject));

        resourceCost.ForEach(r =>
        {
            var item = Instantiate(ItemTemplate);
            item.transform.SetParent(ItemsPanel.transform);
        });

        updateItemCounts();
    }

    private void updateItemCounts()
    {
        int i = 0;
        foreach (var c in activeRecipe.ResourceCost)
        {
            var item = ItemsPanel.GetChild(i).GetComponent<ItemView>();
            item.UpdateUI(c.Resource, PlayerScript.Instance.GetResourceCount(c.Resource) + "/" + c.Amount);
            i++;
        }
    }
}
