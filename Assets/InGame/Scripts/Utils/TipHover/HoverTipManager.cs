using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HoverTipManager : MonoBehaviour
{
    public TMP_Text tipText;
    public RectTransform tipWindow;

    public static Action<string,Vector2> OnMouseHover;
    public static Action OnMouseLoseFocus;
    private void OnEnable() {
        OnMouseHover += ShowTip;
        OnMouseLoseFocus += HideTip;
    }
    private void OnDisable() {
        
        OnMouseHover -= ShowTip;
        OnMouseLoseFocus -= HideTip;
    }
    private void Start() {
        HideTip();
    }
    private void ShowTip(string tip,Vector2 mousePos)
    {
        tipText.text = tip;
        tipWindow.sizeDelta = new Vector2(tipText.preferredHeight > 200 ? 200 : tipText.preferredWidth,tipText.preferredHeight);
        tipWindow.gameObject.SetActive(true);
        tipWindow.transform.position = new Vector2(mousePos.x , mousePos.y);
    }
    private void HideTip()
    {
        tipText.text = default;
        tipWindow.gameObject.SetActive(false);
    }
}
