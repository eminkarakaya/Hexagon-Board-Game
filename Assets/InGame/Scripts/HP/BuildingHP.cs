using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class BuildingHP : HP
{
    public override void Death()
    {
        if(_hp <= 0 )
        {
            Debug.Log(manager.isOwned + "  manager.isOwned  ");
            manager.Capture(GetComponent<NetworkIdentity>());
            TeamColor [] teamColors = GetComponent<Building>().transform.GetComponentsInChildren<TeamColor>();
            foreach (var item in teamColors)
            {
                item.SetColor(manager.data);
            }
            StartCoroutine (GetComponent<Building>().wait(GetComponent<NetworkIdentity>(),GetComponent<Building>().gameObject));
        } 
    }
   
}
