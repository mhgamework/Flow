using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

/// <summary>
/// Generic view for an item + text.
/// Originally copied from InventoryUIStack, so probably InventoryUIStack should be removed and replaced with this
/// </summary>
public class ItemView : MonoBehaviour
{
    [SerializeField]
    private RawImage img;
    [SerializeField]
    private Text text;
    [SerializeField]
    private Text text2;
    [SerializeField]
    private Image PanelImage;

    public bool ShowPanel = true;

    private string resourceType;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PanelImage.enabled = ShowPanel;
    }

    public void UpdateUI(string type, string text)
    {
        UpdateUIInternal(type, text);
    }
    private void UpdateUIInternal(string type, string txt)
    {
        if (resourceType != type)
        {
            resourceType = type;
            updateImage();
        }
        text.text = txt;
        text2.text = txt;
    }

    private void updateImage()
    {
        var type = ResourceTypesScript.Instance().Find(resourceType);
        if (type.UIPrefab == null) throw new InvalidOperationException("No uiprefab found for type: " + type.Identifier);
        var tex = RenderToTextureScript.Instance().PrefabToTexture(type.UIPrefab);
        img.texture = tex;
    }

}
