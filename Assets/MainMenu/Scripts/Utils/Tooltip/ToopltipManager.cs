using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToopltipManager : MonoBehaviour
{
    public static ToopltipManager instance;
    public Tooltip tooltip;
    private void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public static void Show(string content, string header = " ")
    {
        instance.tooltip.SetText(content,header);
        instance.tooltip.gameObject.SetActive(true);
    }
    public static void Hide()
    {
        instance.tooltip.gameObject.SetActive(false);
    }
    
}
