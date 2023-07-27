using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
public enum ResourceType
{
    None,
    Oil,
    Coal,
    Niter,
    Iron
}

public class Resource : NetworkBehaviour
{
    public Mine mine;
    [SerializeField] private CivManager civManager; 
    [SerializeField] private Side side;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private Image resourceIcon,goldIcon;
    private Sprite sprite;
    [SerializeField] private ResourceType resourceType;
    public ResourceType ResourceType{get => resourceType; private set {resourceType = value;}}

    public CivManager CivManager { get => civManager; set{civManager = value;} }
    public Side Side { get => side; set{side = value;} }

    public int Gold;
    public GameObject prefab;
    private void Start() {
        prefab = FindObjectOfType<HexGrid>().GetBuildingPrefab(ResourceType,out sprite);
        if(prefab != null)
        {
            resourceIcon.sprite = sprite;
        }
        if(ResourceType == ResourceType.None)
        {
            resourceIcon.enabled = false;
            goldText.enabled = false;
            goldIcon.enabled = false;
        }
    }
    private void SetGold(int gold)
    {
        Gold = gold;
        goldText.text = Gold.ToString();
    }
   
    [ClientRpc] private void RPCRemoveGold(CivManager civManager)
    {
        RemoveGoldPerTurn(civManager);
    }
    [Command(requiresAuthority = false)] private void CMDRemoveGold(CivManager civManager)
    {
        RPCRemoveGold(civManager);
        TargetSetText(civManager.GetComponent<NetworkIdentity>().connectionToClient,civManager);
    }
    [ClientRpc] private void RPCAddGold(CivManager civManager)
    {
        AddGoldPerTurn(civManager);
    }
    [TargetRpc] private void TargetSetText(NetworkConnectionToClient conn,CivManager civManager)
    {
        civManager.SetGoldPerTurnText();
    }
    [Command(requiresAuthority = false)] public void CMDAddGold(CivManager civManager)
    {
        RPCAddGold(civManager);
        
        TargetSetText(civManager.GetComponent<NetworkIdentity>().connectionToClient,civManager);
    }

    

    public void ChangeOwned(IAttackable attackable)
    {
        // if(civManager.goldTextPerTurn != null)
        CMDRemoveGold(this.CivManager);
        attackable.CivManager.CMDHideAllUnits();
        attackable.CivManager.Capture(mine.GetComponent<NetworkIdentity>());
        TeamColor [] teamColors = mine.transform.GetComponentsInChildren<TeamColor>();
        foreach (var item in teamColors)
        {
            item.SetColor(attackable.CivManager.civData);
        }
        StartCoroutine (mine.wait(attackable.CivManager));
        CMDAddGold(attackable.CivManager);
        
    }
    // public void OpenActive(CivManager civManager)
    // {
    //     CMDOpenActive(civManager);
    // }
    // public void CMDOpenActive(CivManager civManager)
    // {   
    //     RPCOpenActive(civManager);
    // }
    public void RPCOpenActive(CivManager civManager)
    {
        CivManager = civManager;
        
    }
    public void SetSide(Side side, Outline outline)
    {
        this.side = side;
        if(outline == null) return;
        if(side == Side.Me)
        {
            outline.OutlineColor = Color.white;
        }
        else if(side == Side.Enemy)
        {
            outline.OutlineColor = Color.red;
        }
        else
            outline.OutlineColor = Color.blue;

    }
    public void RemoveGoldPerTurn(CivManager civManager)
    {
        civManager.GoldPerTurn -= Gold;

    }
    public void AddGoldPerTurn(CivManager civManager)
    {
        civManager.GoldPerTurn += Gold;
    }
}
