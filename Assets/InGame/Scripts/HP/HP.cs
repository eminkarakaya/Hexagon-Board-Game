using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    public void DefaultDeathAction()
    {
        civManager.DestroyObj(gameObject);
    }
                                // ddamage alan      // damage atan
    public virtual void Death(IDamagable damagable, IAttackable attackable, UnityEvent action = null)
    {   
        if(_hp <= 0 )
        {
            if(action == null)
            {
                civManager.DestroyObj(gameObject);
            }
            else
            {
                action?.Invoke();
            }
        } 
    }
}
