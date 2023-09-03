using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class CivUI : NetworkBehaviour
{
    
    [SerializeField] public CivData civData;
    [SerializeField] SelectCiv selectCiv ;

    private void OnEnable() {
        GetComponent<Image>().sprite = civData.civImage;
    }
    
    public void Select()
    {
        selectCiv = FindObjectOfType<SelectCiv>();
        selectCiv.button.interactable = true;
        selectCiv.selectedImage.sprite = civData.civImage;
        foreach (var item in FindObjectsOfType<PlayerManager>())
        {
            if(item.isOwned)
            {
                item.CMDSetTeam(selectCiv.team);
                item.CMDSetName(selectCiv.nameInputField.text);
                // item.nickname = selectCiv.nameInputField.text;
                GameManager gameManager = FindObjectOfType<GameManager>();
                item.CMDSetCivData(civData.civType,gameManager);
                item.civType = civData.civType;
            
            }
        }
    }
}
