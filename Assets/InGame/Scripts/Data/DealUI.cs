using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]

public class DealUI : MonoBehaviour
{
    public CivManager civManager;
    public Transform parent;
    public List<RelationShipUI> relationShipUIs;
    public TMP_Text civText,relationShipText,userNameText;
    public Button declareWarBtn,declarePeaceBtn;
    public RelationShipUI GetDealUI(CivManager civManager)
    {
        foreach (var item in relationShipUIs)
        {
            if(item.civManager == civManager)
            {
                return item;
            }
        }
        return null;
    }
}
