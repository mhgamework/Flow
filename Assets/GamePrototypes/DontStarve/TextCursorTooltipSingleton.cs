using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TextCursorTooltipSingleton : Singleton<TextCursorTooltipSingleton> {

    public Vector3 Offset = new Vector3();

    private Text text;

    // Use this for initialization
    void Start () {
        text = GetComponent<Text>();

    }

    // Update is called once per frame
    void Update () {

        transform.position = Input.mousePosition + Offset;
	}

    public void SetTooltip(string left, string right)
    {
        var ret = "";
        if (left != null && left != "")
            ret += left + "\n";
        if (right != null && right != "")
            ret += "RMB " + right;

        text.text = ret;
    }

    internal void ClearTooltip()
    {
        SetTooltip(null, null);
    }
}
