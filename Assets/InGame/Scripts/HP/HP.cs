using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
public class HP : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] protected int _hp,_maxHp;
    [SerializeField] protected CivManager civManager;
    public int Hp { get => _hp; set{_hp = value;if(_hp < 0 ) _hp = 0; _hpText.text = _hp.ToString(); } }
    private void Start() {
        if(civManager == null)
            civManager = PlayerManager.FindPlayerManager();
        Hp = _maxHp;
    }
    
    public virtual void Death(IDamagable damagable,IAttackable attackable)
    {   
        if(_hp <= 0 )
        {
            civManager.DestroyObj(gameObject);
        } 
    }
    
}
