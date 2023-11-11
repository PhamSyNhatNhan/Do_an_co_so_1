using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Windfall : SubWeapon
{
    [Header("Details")]
    private Animator amt;
    [SerializeField] private float attackDamge;
    [SerializeField] private float attackDamgeAir;
    [SerializeField] private float knockBack;
    private NewBehaviourScript pc; 
    
    private bool GotInput; 
    [SerializeField] private bool IsAttack; 
    private int AttackNumber; 
    private int MaxAttackNumber = 1; 
    private float AttackTran = 0.5f; 
    private float LastInputTime = Mathf.NegativeInfinity;
    private float[] AttackDetail = new float[3]; 
    [SerializeField] private LayerMask WhatIsDamgeEnable;
    
    [Header("Hitbox")]
    [SerializeField] private Transform AttackHitBox; 
    [SerializeField] private float AtackRadius; 
    [SerializeField] private Transform AirAttackHitBox;
    [SerializeField] private float AirAtackRadius;

    [Header("Effect")]
    [SerializeField] private GameObject AirAttack_Effect1;
    [SerializeField] private GameObject AirAttack_Effect2;
    private bool IsAirAttack = false;
    
    [SerializeField] private bool IsAirAttack_Bet = false;
    [SerializeField] private float AirAttackDistance_Bet;
    [SerializeField] private Transform AirAttack_Bet;
    [SerializeField] private LayerMask WhatisGround;

    [Header("Burst")] 
    [SerializeField] private float burstCD;
    [SerializeField] private float burstBonusDamage;
    [SerializeField] private float burstBonusAirDamage;
    [SerializeField] private float burstTime;
    [SerializeField] private float burstJumpForce;
    [SerializeField] private GameObject burstAnimation;
    private Echo echo;
    private bool isBurst = false;
    private float lastBurstTime = -100.0f;
    private Player_Stat ps;
        
    private void Start()
    {
        amt = GetComponent<Animator>();
        pc = GameObject.Find("Player").GetComponent<NewBehaviourScript>();
        AttackNumber = 1;
        echo = GameObject.Find("PlayerTrail").GetComponent<Echo>();
        ps = GameObject.Find("Player").GetComponent<Player_Stat>();
    }

    private void Update()
    {
        if (base.IsPickUp == false) return; 
        
        amt.SetBool("IsPickUp", base.IsPickUp);
        CheckCombatInput(); 
        Attack(); 
        ResetAttack();
        SubAttack();
        SubAirAttack();
        subBurst();
    }
    
    private void PickUp(Transform tf)
    {
        base.PickUp(tf); 
    }

    private void Drop()
    {
        echo.DisableTrail();
        isBurst = false;
        AttackNumber = 1;
        deBurstChange();
        
        base.Drop();
        
        amt.SetBool("IsPickUp", base.IsPickUp); 
    }
    
    private void CheckCombatInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GotInput = true;
            LastInputTime = Time.time;
        }

        if (Input.GetKey(KeyCode.Q) && Time.time >= (lastBurstTime + burstCD) && isBurst == false)
        {
            isBurst = true;
            lastBurstTime = Time.time;
            echo.EnableTrail();
            enBurstChange();
            Instantiate(burstAnimation, pc.transform);
        }
    }

    private void Attack()
    {
        if(GotInput && !pc.GetIsDash() && !pc.GetIsWallSlide()) 
        {
            if (!IsAttack) 
            {
                GotInput = false;
                IsAttack = true;

                if (!pc.getIsGrounded())
                {
                    amt.SetBool("IsAttack", true);
                    amt.SetBool("AirAttack", true);
                    CanAirAttackBetter();
                    
                    pc.GetRb().velocity = new Vector2(0, -60f);
                    IsAirAttack = true;
                    
                    pc.GetRb().velocityX = 0.0f;
                    pc.DisableInput();
                }
                else if (pc.getIsGrounded())
                {
                    amt.SetBool("IsAttack", IsAttack);
                    amt.SetBool("Attack", true);
                    
                    AttackNumber += 1;
                    if (AttackNumber > MaxAttackNumber)
                    {
                        AttackNumber = 1;
                    }
                    
                    Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AttackHitBox.position, AtackRadius, WhatIsDamgeEnable);

                    if (!isBurst)
                    {
                       AttackDetail[0] = attackDamge; 
                    }
                    else if (isBurst)
                    {
                        AttackDetail[0] = attackDamge + burstBonusDamage;
                    }
                    
                    AttackDetail[1] = GameObject.Find("Player").transform.position.x; 
                    AttackDetail[2] = knockBack;
        
                    foreach(Collider2D Colider in DetectObject)
                    {
                        Colider.transform.SendMessage("Damage", AttackDetail);
                    }
                }
            }
        }
    }

    private void EndAttack()
    {
        IsAttack = false;
        amt.SetBool("Attack", false);
        amt.SetBool("IsAttack", false);
        amt.SetBool("AirAttack", false);
        GameObject.Find("Player").GetComponent<Player_Weapion_Manager>().SubAttackDisable();
    }
    
    private void ResetAttack()
    {
        if (Time.time >= LastInputTime + AttackTran)
        {
            AttackNumber = 1;
        }

        if (AttackNumber > MaxAttackNumber)
        {
            AttackNumber = 1;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(AttackHitBox.position, AtackRadius);
        Gizmos.DrawWireSphere(AirAttackHitBox.position, AirAtackRadius);
        Gizmos.DrawRay(AirAttack_Bet.position, -transform.up * AirAttackDistance_Bet);
    }
    
    private void SubAttack()
    {
        if (IsAttack)
        {
            GameObject.Find("Player").GetComponent<Player_Weapion_Manager>().SubAttackEnable();
        }
    }

    private void CanAirAttackBetter()
    {
        IsAirAttack_Bet = !(Physics2D.Raycast(AirAttack_Bet.position, -transform.up, AirAttackDistance_Bet, WhatisGround));
    }

    private void SubAirAttack()
    {
        if(IsAirAttack)
        {
            if (pc.getIsGrounded())
            {
                IsAirAttack = false;
                IsAttack = false;
                amt.SetBool("AirAttack", false);
                amt.SetBool("IsAttack", false);
                pc.EnableInput();

                if (IsAirAttack_Bet)
                {
                    Instantiate(AirAttack_Effect1, pc.transform);
                    Instantiate(AirAttack_Effect2, pc.transform);
                
                    AttackNumber += 1;
                    if (AttackNumber > MaxAttackNumber)
                    {
                        AttackNumber = 1;
                    }
                    
                    Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AirAttackHitBox.position, AirAtackRadius, WhatIsDamgeEnable);

                    if (!isBurst)
                    {
                        AttackDetail[0] = attackDamgeAir; 
                    }
                    else if (isBurst)
                    {
                        AttackDetail[0] = attackDamgeAir + burstBonusAirDamage;
                    }
                    AttackDetail[1] = GameObject.Find("Player").transform.position.x; 
                    AttackDetail[2] = knockBack;
        
                    foreach(Collider2D Colider in DetectObject)
                    {
                        Colider.transform.SendMessage("Damage", AttackDetail);
                    }
                    
                    GameObject.Find("Player").GetComponent<Player_Weapion_Manager>().SubAttackDisable();
                }
                else if(!IsAirAttack_Bet)
                {
                    Instantiate(AirAttack_Effect1, pc.transform);
                
                    AttackNumber += 1;
                    if (AttackNumber > MaxAttackNumber)
                    {
                        AttackNumber = 1;
                    }
                    
                    Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AirAttackHitBox.position, AirAtackRadius, WhatIsDamgeEnable);

                    if (!isBurst)
                    {
                        AttackDetail[0] = attackDamge; 
                    }
                    else if (isBurst)
                    {
                        AttackDetail[0] = attackDamge + burstBonusAirDamage - 1;
                    }
                    
                    AttackDetail[1] = GameObject.Find("Player").transform.position.x; 
                    AttackDetail[2] = 0;
        
                    foreach(Collider2D Colider in DetectObject)
                    {
                        Colider.transform.SendMessage("Damage", AttackDetail);
                    }
                    
                    GameObject.Find("Player").GetComponent<Player_Weapion_Manager>().SubAttackDisable();
                }
            }
        }
    }


    private void AirAttackFinish()
    {
        pc.EnableInput();
    }
    
    private void subBurst()
    {
        if (isBurst)
        {
            if (Time.time >= lastBurstTime + burstTime)
            {
                isBurst = false;
                echo.DisableTrail();
                deBurstChange();
            }
        }
    }

    private void enBurstChange()
    {
        ps.SetJumpForce(burstJumpForce);
        ps.SetAmountJump(1);
        ps.SetJumpvar(1);
        pc.GetRb().gravityScale = 4.0f;
    }
    private void deBurstChange()
    {
        ps.SetJumpForce(20.0f);
        ps.SetAmountJump(2);
        ps.SetJumpvar(0.5f);
        pc.GetRb().gravityScale = 5.0f;
    }
}
