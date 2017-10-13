using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class InventoryUIStack : MonoBehaviour
{
    [SerializeField]
    private RawImage img;
    [SerializeField]
    private Text text;
    [SerializeField]
    private Text text2;

    private string resourceType;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateUI(string type, int amount)
    {
        UpdateUI(new InventoryScript.InventoryItem(type, amount));
    }
    private void UpdateUIInternal(string type, int amount)
    {
        if (resourceType != type)
        {
            resourceType = type;
            updateImage();
        }
        text.text = amount.ToString();
        text2.text = amount.ToString();
    }

    private void updateImage()
    {
        var type = ResourceTypesScript.Instance().Find(resourceType);
        if (type.UIPrefab == null) throw new InvalidOperationException("No uiprefab found for type: " + type.Identifier);
        var tex = RenderToTextureScript.Instance().PrefabToTexture(type.UIPrefab);
        img.texture = tex;
    }

    public void UpdateUI(InventoryScript.InventoryItem s)
    {
        if (s.IsEmpty)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
            UpdateUIInternal(s.ResourceType, s.Amount);

        }
    }
}
