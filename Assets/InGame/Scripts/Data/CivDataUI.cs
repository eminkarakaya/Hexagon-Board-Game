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
            _civManager.CMDDeclareWar(civManager.gameObject,_civManager);
            _civManager.CMDDeclareWar(_civManager.gameObject,civManager);
            _civManager.CMDDeclareWar2(civManager);
            
            CMDDeclareWar(_civManager);
            CMDDeclareWar(civManager);
        });
        dealUI.declarePeaceBtn.onClick.AddListener(()=>
        {
            CMDDeclarePeace(civManager);
            CMDDeclarePeace(_civManager);
            _civManager.CMDDeclarePeace(civManager.gameObject,_civManager);
            _civManager.CMDDeclarePeace(_civManager.gameObject,civManager);
            _civManager.CMDDeclarePeace2(civManager);
        });
    }
    [Command(requiresAuthority = false)]  public void CMDDeclarePeace(CivManager civManager)
    {
        RPCDeclarePeace(civManager);
    }
    [Command(requiresAuthority = false)]  public void CMDDeclareWar(CivManager civManager)
    {
        RPCDeclareWar(civManager);
    }
    [ClientRpc] public void RPCDeclarePeace(CivManager civManager)
    {
        foreach (var item in civManager.civDataUI.dealUI.relationShipUIs)
        {
            if(civManager.savastigiCivler.Contains(item.civManager))
            {
                Debug.Log(item+ " item ",item);
                item.relationShipImage.sprite = GameSettingsScriptable.Instance.notrSprite;
            }
        }
    }
    [ClientRpc] public void RPCDeclareWar(CivManager civManager)
    {
        foreach (var item in civManager.civDataUI.dealUI.relationShipUIs)
        {
            if(civManager.savastigiCivler.Contains(item.civManager))
            {
                item.relationShipImage.sprite = GameSettingsScriptable.Instance.warSprite;
            }
        }
    }
   
    public void SetNicknameText()
    {
        dealUI = Instantiate(dealUIPrefab);
        dealUI.civManager = civManager;
        foreach (var item in FindObjectsOfType<CivManager>())
        {
            var ui = Instantiate(GameSettingsScriptable.Instance.RelationShipsPrefab.gameObject,dealUI.parent).GetComponent<RelationShipUI>();
            ui.civManager = item;
            ui.civImage.sprite = item.civData.civImage;
            if(item.Side == Side.Ally)
            {
                ui.relationShipImage.sprite = GameSettingsScriptable.Instance.allySprite;
            }
            else if(item.Side == Side.Enemy)
            {
                ui.relationShipImage.sprite = GameSettingsScriptable.Instance.warSprite;
            }

            dealUI.relationShipUIs.Add(ui);
        }
        civManager.civDataUI = this;
        hoverTip.GetComponent<HoverTip>().tipToShow = civManager.nickname;
        dealUI.civText.text = civData.civName;
        dealUI.userNameText.text = civManager.nickname;
        
        if(civManager.isOwned)
        {
            dealUI.declareWarBtn.interactable = false;
        }
    }  
}
