using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
public class HP : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private int _hp,_maxHp;
    [SerializeField] protected CivManager manager;
    public int Hp { get => _hp; set{_hp = value; _hpText.text = _hp.ToString();} }
    private void Start() {
        if(manager == null)
            manager = PlayerManager.FindPlayerManager();
        Hp = _maxHp;
    }
    
    public virtual void Death()
    {   
        if(_hp <= 0 )
        {
            manager.DestroyObj(gameObject);
        } 
    }
    public virtual void Death(NetworkIdentity identity)
    {   
        if(_hp <= 0 )
        {
            manager.DestroyObj(gameObject);
        } 
    }
    
}
