using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmiyaCut : MonoBehaviour
{
    [SerializeField] private LayerMask WhatIsDamgeEnable;
    [SerializeField] private Transform AttackHitBox; 
    [SerializeField] private float AtackRadius;
    private float[] AttackDetail = new float[3];
    private Animya amiya;
    
    void Start()
    {
        amiya = GameObject.Find("Amiya").GetComponent<Animya>();
        EnchanceCut();
    }

    private void EnchanceCut()
    {
        if (amiya.GetIsBurst())
        {
            AtackRadius *= 1.5f;
        }
        Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AttackHitBox.position, AtackRadius, WhatIsDamgeEnable);
        
        if (amiya.GetIsBurst())
        {
            AttackDetail[0] = amiya.GetAmiyaCutDamage() + amiya.GetAttackDamageBonus();
        }
        else
        {
            AttackDetail[0] = amiya.GetAmiyaCutDamage();
        }
        AttackDetail[1] = GameObject.Find("Player").transform.position.x; 
        AttackDetail[2] = amiya.GetKnockBack();
        
        foreach(Collider2D Colider in DetectObject)
        {
            Colider.transform.SendMessage("Damage", AttackDetail);
        }
    }

    private void resetInput()
    {
        amiya.resestInput();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(AttackHitBox.position, AtackRadius);
    }
}
