using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class InventoryWindowScript : MonoBehaviour
{



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowInventory(InventoryScript a, string nameA, InventoryScript b, string nameB)
    {
        try
        {
            //TODO:
            transform.GetChild(0).gameObject.SetActive(true);

            GetComponentsInChildren<Text>().First(t => t.name == "TitleA").text = nameA;
            GetComponentsInChildren<Text>().First(t => t.name == "TitleB").text = nameB;
            GetComponentsInChildren<InteractableInventoryUIScript>().First(t => t.name == "InventoryA").SetInventory( a);
            GetComponentsInChildren<InteractableInventoryUIScript>().First(t => t.name == "InventoryB").SetInventory( b);
        }
        catch (Exception)
        {
            
            throw;
        }
      

    }

    public void HideInventory()
    {

        transform.GetChild(0).gameObject.SetActive(false);
    }

}
