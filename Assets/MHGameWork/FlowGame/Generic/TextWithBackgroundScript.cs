using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MHGameWork.FlowGame.Generic
{
    public class TextWithBackgroundScript : MonoBehaviour
    {
        [SerializeField] private Color backgroundColor = Color.black;

        [SerializeField] private float offset = 1.2f;

        [SerializeField] private TextWithBackgroundScriptStyle style = TextWithBackgroundScriptStyle.Shadow;

        private Text oriText;

        private List<Text> shadowTexts = new List<Text>();
        private List<Vector2> offsets = new List<Vector2>();

        public void Start()
        {
            oriText = GetComponent<Text>();
            if (style == TextWithBackgroundScriptStyle.Shadow)
            {
                createTextClone(oriText, offset, offset);
                createTextClone(oriText, -offset, offset);
                createTextClone(oriText, -offset, -offset);
                createTextClone(oriText, offset, -offset);
            }

            if (style == TextWithBackgroundScriptStyle.Border)
            {
                createTextClone(oriText, offset, offset);
            }

            OnValidate();
        }

        private void createTextClone(Text baseText, float offsetX, float offsetY)
        {
            var g = new GameObject(oriText.gameObject.name + "-ShadowText", typeof(Text));

            var ret = g.GetComponent<Text>();
            ret.rectTransform.parent = oriText.rectTransform.parent;
            ret.rectTransform.SetSiblingIndex(oriText.rectTransform.GetSiblingIndex());
            ret.fontSize = oriText.fontSize;
            ret.font = oriText.font;
            ret.alignment = oriText.alignment;
            ret.rectTransform.anchorMin = oriText.rectTransform.anchorMin;
            ret.rectTransform.anchorMax = oriText.rectTransform.anchorMax;
            ret.rectTransform.sizeDelta = oriText.rectTransform.sizeDelta;

            shadowTexts.Add(ret);
            offsets.Add(new Vector2(offsetX, offsetY));
        }

        private void OnValidate()
        {
            if (shadowTexts == null) return;
            for (int i = 0; i < shadowTexts.Count; i++)
            {
                shadowTexts[i].color = backgroundColor;
            }
        }


        public void Update()
        {
            for (int i = 0; i < shadowTexts.Count; i++)
            {
                var text = shadowTexts[i];
                var offset = offsets[i];
                text.text = oriText.text;
                text.rectTransform.anchoredPosition3D =oriText.rectTransform.anchoredPosition + new Vector2(offset.x, -offset.y);
            }

        }

        public enum TextWithBackgroundScriptStyle
        {
            Shadow,
            Border
        }
    }
}