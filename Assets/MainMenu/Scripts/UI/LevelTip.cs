using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[RequireComponent(typeof(TooltipTrigger))]
public class LevelTip : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    TooltipTrigger tooltipTrigger;
    public int level;
    private void Start() {
        tooltipTrigger = GetComponent<TooltipTrigger>();
    }
    public void SetHoverTipText()
    {
        tooltipTrigger.content = "Level";
    }

    
}
