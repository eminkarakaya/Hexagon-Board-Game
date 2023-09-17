using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
public class Melee : Attack
{
    
    private void Start() {
        UpdateAnimClipTimes();
    }
    public void UpdateAnimClipTimes()
    {
        if(networkAnimator == null) return;
        AnimationClip[] clips = networkAnimator.animator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            switch(clip.name)
            {
                case "SwordAttackAnim":
                    attackTime = clip.length/networkAnimator.animator.GetFloat("AttackAnimationSpeed");
                    break;
            }
        }
    }
    public override IEnumerator AttackUnit(IDamagable damagable,IAttackable attackable,float movementDuration)
    {
        if(GetComponent<Movement>().GetCurrentMovementPoints()==0) yield break;
        // if(damagable.Hex.IsWater()) yield break;
        float timeElapsed = 0f;
        while(timeElapsed<movementDuration)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / movementDuration;
            transform.position = Vector3.Lerp(transform.position,TransformUtils.FixY(damagable.Hex.transform.position),lerpStep);
            yield return null;
        }

        StartCoroutine(OpenTransparentMaterial(attackTime/2));
        networkAnimator.SetTrigger("Attack");
        timeElapsed = 0f;
        while(timeElapsed<attackTime )
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        int hp  = damagable.hp.Hp;
        
        StartCoroutine(CloseTransparentMaterial(attackTime/2));

        // hasarın uygulandıgı yer
        if(isServer)
        {
            RPCAttack(damagable.hp);            
        }
        else
        {
            CMDAttack(damagable.hp);
        }
        // serverden mesaj gelene kadar beklıyor
        while(hp == damagable.hp.Hp)
        {
            yield return null;
        }
        // kill events
        foreach (var item in PropertiesEnumList)
        {

            if(item.attackPropertiesEnum == PropertiesEnum.MoveKill)
            {
                if(TryGetComponent(out UnitMovement movement0))
                {
                    if(damagable.hp.Hp<=0)
                        StartCoroutine (movement0.MoveKill(damagable.Hex,damagable.hp.Hp<=0));
                    else
                    {
                            timeElapsed = 0f;
                        while(timeElapsed<movementDuration)
                        {
                            timeElapsed += Time.deltaTime;
                            float lerpStep = timeElapsed / movementDuration;
                            transform.position = Vector3.Lerp(transform.position,TransformUtils.FixY(attackable.Hex.transform.position),lerpStep);
                            yield return null;
                        }
                    }
                }
            }
            if(item.attackPropertiesEnum == PropertiesEnum.TakeHostage)
            {
                if(TryGetComponent(out UnitMovement movement1))
                {
                    if(damagable.hp.Hp<=0)
                    {
                        movement1.TakeHostage(damagable.Hex,damagable.hp.Hp<=0);
                    }
                    else
                    {
                            timeElapsed = 0f;
                        while(timeElapsed<movementDuration)
                        {
                            timeElapsed += Time.deltaTime;
                            float lerpStep = timeElapsed / movementDuration;
                            transform.position = Vector3.Lerp(transform.position,TransformUtils.FixY(attackable.Hex.transform.position),lerpStep);
                            yield return null;
                        }
                    }
                }
            }
            
        }
        damagable.hp.Death(damagable,attackable,true);
    }
    [Command]
    protected void CMDAttack(HP hp)
    {
        RPCAttack(hp);
    }
    [ClientRpc]
    public void RPCAttack(HP hp)
    {
        if(hp != null)
        {
            hp.Hp -= _damagePower;

        }

        // InflictDamage(hp);
    }
}
