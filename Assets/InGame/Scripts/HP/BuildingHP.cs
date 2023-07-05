using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class BuildingHP : HP
{
    public override void Death(IDamagable damagable, IAttackable attackable)
    {
        if(_hp <= 0 )
        {
            
            attackable.CivManager.CMDHideAllUnits();
            attackable.CivManager.Capture(GetComponent<NetworkIdentity>());
            TeamColor [] teamColors = GetComponent<Building>().transform.GetComponentsInChildren<TeamColor>();
            foreach (var item in teamColors)
            {
                item.SetColor(attackable.CivManager.civData);
            }
            StartCoroutine (GetComponent<Building>().wait(GetComponent<NetworkIdentity>(),GetComponent<Building>().gameObject,attackable.CivManager));
        } 
    }
   
}
