using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmiyaBurst : MonoBehaviour
{
    [SerializeField] private LayerMask WhatIsDamgeEnable;
    [SerializeField] private Transform AttackHitBox; 
    [SerializeField] private float AtackRadius;
    private float[] AttackDetail = new float[3];
    private Animya amiya;
    
    // Start is called before the first frame update
    void Start()
    {
        amiya = GameObject.Find("Amiya").GetComponent<Animya>();
    }

    private void BurstDamage()
    {
        Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AttackHitBox.position, AtackRadius, WhatIsDamgeEnable);

        AttackDetail[0] = amiya.getBurstBaseDamage();
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
