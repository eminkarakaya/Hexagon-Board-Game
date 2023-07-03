using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
public abstract class Attack : NetworkBehaviour
{
    protected IAttackable attackable;
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] protected int _damagePower;
    public System.Action AttackEvent;
    public System.Action<Hex> KillEvent;
    protected int DamagePower { get; set; }
    public int range = 1;
    private void Start() {
        SetDamage(_damagePower);
        attackable = GetComponent<IAttackable>();
    }
    void SetDamage(int value)
    {
        _damagePower = value;
        _damageText.text = _damagePower.ToString();
    }
    
    public abstract void AttackUnit(IDamagable damagable);
    // [Command]
    // protected virtual void CMDInflictDamage(HP hp)
    // {
    //     InflictDamage(hp);
    // } 

    protected void InflictDamage(HP hp)
    {
        
    }

}
