using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
public enum ColorType
{
    Primary,
    Secondary
}
public class TeamColor : MonoBehaviour
{
    
    public ColorType colorType;
    public void SetColor(CivData civData)
    {
        if(colorType == ColorType.Primary)
        {
            if(TryGetComponent(out TextMeshProUGUI text))
            {
                text.color = civData.primaryColor;
            }
            else
            {
                GetComponent<Image>().color = civData.primaryColor;
            }
        }
        else
        {
            if(TryGetComponent(out TextMeshProUGUI text))
            {

                text.color = civData.secondaryColor;
            }
            else
                GetComponent<Image>().color = civData.secondaryColor;
        }
    }
}
