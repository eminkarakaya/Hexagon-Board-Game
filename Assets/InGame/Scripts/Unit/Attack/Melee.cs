using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
public class Melee : Attack
{
    public float attackTime;
    private AnimationClip clip;
    [SerializeField] private float moveDur;
    private void Start() {
        UpdateAnimClipTimes();
    }
    public void UpdateAnimClipTimes()
    {
        AnimationClip[] clips = networkAnimator.animator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            switch(clip.name)
            {
                case "SwordAttackAnim":
                    attackTime = clip.length;
                    break;
            }
        }
    }
    public override IEnumerator AttackUnit(IDamagable damagable,IAttackable attackable,float movementDuration)
    {
        if(GetComponent<Movement>().GetCurrentMovementPoints()==0) yield break;
        if(damagable.Hex.IsWater()) yield break;
        
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
        bool isHalf = false;
        while(timeElapsed<attackTime)
        {
            timeElapsed += Time.deltaTime;
            if(timeElapsed > attackTime/2 && !isHalf)
            {
                isHalf = true;
            }
            yield return null;
        }
        
                StartCoroutine(CloseTransparentMaterial(attackTime/2));
        if(isServer)
        {
            Attack(damagable.hp);            
        }
        else
        {
            CMDAttack(damagable.hp);
        }

        // kill events
        foreach (var item in funcs)
        {

            if(item == Funcs.MoveKill)
            {
                if(TryGetComponent(out UnitMovement movement0))
                {
                    StartCoroutine (movement0.MoveKill(damagable.Hex,damagable.hp.Hp<=0));
                }
            }
            if(item == Funcs.TakeHostage)
            {
                if(TryGetComponent(out UnitMovement movement1))
                {
                    movement1.TakeHostage(damagable.Hex,damagable.hp.Hp<=0);
                }
            }
        }
        damagable.hp.Death(damagable,attackable,killEvent);
        if(damagable.hp.Hp > 0)
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
    [Command]
    protected void CMDAttack(HP hp)
    {
        Attack(hp);
    }
    [ClientRpc]
    public void Attack(HP hp)
    {
        if(hp != null)
        {
            hp.Hp -= _damagePower;

        }

        // InflictDamage(hp);
    }
}
