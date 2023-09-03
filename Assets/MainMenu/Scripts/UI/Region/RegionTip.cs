using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(TooltipTrigger))]
public class RegionTip : MonoBehaviour
{
    Image image;
    private TooltipTrigger tooltipTrigger;
    private RegionData regionData;
    [HideInInspector] public RegionType regionType;
  
    public void InitializeTip() {
        tooltipTrigger = GetComponent<TooltipTrigger>();
        image = GetComponent<Image>();
        regionData = Region.GetRegionDataByRegionType(regionType);
        image.sprite = regionData.regionSprite;        
    }
    public void SetHoverTipText()
    {
        tooltipTrigger.content = regionType.ToString();
    }
}
