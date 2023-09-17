using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Mirror;
public class HP : NetworkBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] protected int _hp,_maxHp;
    [SerializeField] protected CivManager civManager;
    public int Hp { get => _hp; set{_hp = value; _hpText.text = _hp.ToString(); slider.value = _hp; } }
    private void Start() {
        if(civManager == null)
            civManager = PlayerManager.FindPlayerManager();
        Hp = _maxHp;
        slider.maxValue = Hp;
        slider.value = Hp;
    }
    [Command(requiresAuthority = false)] public void CMDRemoveOrderList()
    {
        RemoveOrderListTargetRpc();
    }
    [TargetRpc] private void RemoveOrderListTargetRpc()
    {
        if(Hp<=0)
        {
            if(TryGetComponent(out ITaskable taskable))
            {
                civManager.RemoveOrderList(taskable);
            }

        }
    }
                                // ddamage alan      // damage atan
                                // öldüren objede çalışıyor
    public virtual void Death(IDamagable damagable, IAttackable attackable, bool kill,UnityEvent action = null)
    {   
        if(Hp <= 0 )
        {
            if(action == null)
            {

            }
            else
            {
                action?.Invoke();
                
            }
            
            if(kill == true)
            {
                // CMDRemoveOrderList();
                civManager.DestroyObj(damagable.hp.gameObject);
                
            }
        } 
    }
    public virtual void TakeHostage(IDamagable damagable, IAttackable attackable, UnityEvent action = null)
    {

    }
}
