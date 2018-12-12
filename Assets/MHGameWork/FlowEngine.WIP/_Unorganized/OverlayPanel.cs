using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayPanel : Singleton<OverlayPanel>
{
    private Text textComponent;
    private GameObject panelObject;
    private Image panelImageComponent;
    private CanvasGroup canvasGroup;

    // Use this for initialization
    void Start()
    {
        textComponent = GetComponentInChildren<Text>(true);
        panelImageComponent = GetComponentInChildren<Image>(true);
        canvasGroup = GetComponentInChildren<CanvasGroup>(true);
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

    public void Hide(string text, float fadeoutTime)
    {
        textComponent.text = text;
        StartCoroutine(hideFadeout(fadeoutTime).GetEnumerator());

    }

    public IEnumerable<YieldInstruction> hideFadeout(float fadeoutTime)
    {
        var time = 0f; 
        while (time < fadeoutTime)
        {
            canvasGroup.alpha = EasingFunction.EaseInQuart(0, 1, (fadeoutTime - time) / fadeoutTime);
            yield return null;
            time += Time.deltaTime;
        }
        Hide();
        canvasGroup.alpha = 1;
    }
}
