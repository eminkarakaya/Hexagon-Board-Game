using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class BuildingHP : HP
{
    public override void Death(IDamagable damagable)
    {
        if(_hp <= 0 )
        {
            if(civManager == null)
            {
                civManager = PlayerManager.FindPlayerManager();
            }
            civManager.Capture(GetComponent<NetworkIdentity>());
            TeamColor [] teamColors = GetComponent<Building>().transform.GetComponentsInChildren<TeamColor>();
            foreach (var item in teamColors)
            {
                item.SetColor(civManager.civData);
            }
            StartCoroutine (GetComponent<Building>().wait(GetComponent<NetworkIdentity>(),GetComponent<Building>().gameObject,damagable.CivManager));
        } 
    }
   
}
