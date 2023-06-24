using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
public class HP : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private int _hp,_maxHp;
    
    public int Hp { get => _hp; set{_hp = value; _hpText.text = _hp.ToString();} }
    private void Start() {
        Hp = _maxHp;
    }
    public void Death()
    {   
        if(_hp <= 0 )
        {
            PlayerManager manager = FindObjectOfType<PlayerManager>();
            manager.DestroyObj(gameObject);
            KillUnit?.Invoke();
        } 
    }
    public System.Action KillUnit;
}
