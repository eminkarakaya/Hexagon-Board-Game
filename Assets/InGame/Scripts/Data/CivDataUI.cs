using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System;
using System.Linq;

public class CivDataUI : NetworkBehaviour
{
    public DealUI dealUIPrefab;
    public Image civImage;
    public CivManager civManager;
    public CivData civData;
    [SerializeField] GameObject hoverTip;
    public DealUI dealUI;
    List<DealUI> dealUIs = new List<DealUI>();
    public void OpenUI(bool state)
    {
        if(dealUIs.Count == 0)
            dealUIs = FindObjectsOfType<DealUI>().ToList();
        foreach (var item in dealUIs)
        {
            item.gameObject.SetActive(false);
        }
        dealUI.gameObject.SetActive(state);
    }
    public void SetDeclareWarButton(CivManager _civManager)
    {
        dealUI.declareWarBtn.onClick.AddListener( ()=>
        {
            civManager.CMDDeclareWar(civManager.gameObject,_civManager);
            civManager.CMDDeclareWar(_civManager.gameObject,civManager);
        });
        dealUI.declarePeaceBtn.onClick.AddListener(()=>
        {
            civManager.CMDDeclarePeace(civManager.gameObject,_civManager);
            civManager.CMDDeclarePeace(_civManager.gameObject,civManager);
            
        });
    }
    
    public void SetNicknameText()
    {
        dealUI = Instantiate(dealUIPrefab);
        civManager.civDataUI = this;
        hoverTip.GetComponent<HoverTip>().tipToShow = civManager.nickname;
        dealUI.civText.text = civData.civName;
        dealUI.userNameText.text = civManager.nickname;
        dealUI.relationShipText.text = "Friendly";
        
        if(civManager.isOwned)
        {
            dealUI.declareWarBtn.interactable = false;
        }
    }  
}
