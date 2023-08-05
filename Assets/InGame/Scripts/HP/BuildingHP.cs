using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class BuildingHP : HP
{
    public override void Death(IDamagable damagable, IAttackable attackable, UnityEvent action = null)
    {
        if(Hp <= 0 )
        {
            if(action != null)
            {
                action?.Invoke();
            }

            attackable.CivManager.CMDHideAllUnits();
            attackable.CivManager.Capture(GetComponent<NetworkIdentity>());
            TeamColor [] teamColors = GetComponent<Building>().transform.GetComponentsInChildren<TeamColor>();
            foreach (var item in teamColors)
            {
                item.SetColor(attackable.CivManager.civData);
            }
            StartCoroutine (GetComponent<Building>().wait(attackable.CivManager));
        } 
        // StartCoroutine(Wait(damagable,attackable));
    }
}
