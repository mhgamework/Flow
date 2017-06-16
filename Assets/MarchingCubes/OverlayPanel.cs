using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayPanel : Singleton<OverlayPanel>
{
    private Text textComponent;
    private GameObject panelObject;
    private Image panelImageComponent;

    // Use this for initialization
    void Start()
    {
        textComponent = GetComponentInChildren<Text>();
        panelImageComponent = GetComponentInChildren<Image>();
        panelObject = panelImageComponent.gameObject;

        Hide();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(string text)
    {
        textComponent.text = text;
        panelObject.SetActive(true);

    }

    public void Hide()
    {
        panelObject.SetActive(false);

    }

    public void Hide(string text, int fadeoutTime)
    {
        textComponent.text = text;
        StartCoroutine(hideFadeout(fadeoutTime).GetEnumerator());

    }

    public IEnumerable<YieldInstruction> hideFadeout(int fadeoutTime)
    {
        var startColor = panelImageComponent.color;
        var endColor = panelImageComponent.color;
        endColor.a = 0;
        var time = 0f; 
        while (time < fadeoutTime)
        {
            panelImageComponent.color = Color.Lerp(endColor,startColor , EasingFunction.EaseInQuart(0, 1, (fadeoutTime - time) / fadeoutTime));
            yield return null;
            time += Time.deltaTime;
        }
        Hide();
        panelImageComponent.color = startColor;
    }
}
