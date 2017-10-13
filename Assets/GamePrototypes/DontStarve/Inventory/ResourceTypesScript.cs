using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using Assets.DontStarve.Inventory;

public class ResourceTypesScript : MonoBehaviour
{
    [SerializeField]
    private List<ResourceType> Types;
    private Dictionary<string, ItemType> itemTypes = new Dictionary<string, ItemType>();

    public static ResourceTypesScript Instance()
    {
        var ret = FindObjectOfType<ResourceTypesScript>();
        if (!ret) Debug.LogError("No instance of RenderToTextureScript found in scene!");
        return ret;
    }

    // Use this for initialization
    void Start()
    {
        ItemType t ;

        t= new ItemType("Red Mushroom");
        t.SetEdible(-0.2f, -0.1f);
        t.SetCookable("cookedredmushroom");
        itemTypes.Add("redmushroom", t);

        t = new ItemType("Cooked Red Mushroom");
        t.SetEdible(0.2f, -0.1f);
        itemTypes.Add("cookedredmushroom", t);

        t = new ItemType("Brown Mushroom");
        t.SetEdible(0.1f, -0.2f);
        t.SetCookable("cookedbrownmushroom");
        itemTypes.Add("brownmushroom", t);

        t = new ItemType("Cooked Brown Mushroom");
        t.SetEdible(-0.1f, 0.2f);
        itemTypes.Add("cookedbrownmushroom", t);

        t = new ItemType("Axe");
        itemTypes.Add("axe", t);

        t = new ItemType("Logs");
        t.SetFuel(1);
        itemTypes.Add("logs", t);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public ResourceType Find(string identifier)
    {
        return Types.FirstOrDefault(t => t.Identifier == identifier);
    }

    public ItemType GetItemTypeForIdentifier(string identifier)
    {
        if (!itemTypes.ContainsKey(identifier)) return new ItemType("UNKNOWN - " + identifier);
        return itemTypes[identifier];
    }

}
