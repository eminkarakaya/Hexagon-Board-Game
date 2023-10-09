using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TooltipTrigger))]
public class PropertyTip : MonoBehaviour
{
    Image image;
    private TooltipTrigger toolTipTrigger;
    private PropertiesData propertiesData;
    [HideInInspector] public PropertiesEnum propertiesEnum;
    private void OnValidate() {
    }
    public void InitializeTip() {
        toolTipTrigger = GetComponent<TooltipTrigger>();
        image = GetComponent<Image>();
        propertiesData = Properties.GetRegionDataByPropType(propertiesEnum);
        image.sprite = propertiesData.propertySprite;        
    }
    public void SetHoverTipText()
    {
        toolTipTrigger.header = propertiesData.propertiesEnum.ToString();
        toolTipTrigger.content = propertiesData.propTip.ToString();
    }
}
