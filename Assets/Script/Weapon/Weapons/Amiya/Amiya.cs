using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Animya : SubWeapon
{
    [Header("Details")]
    private Animator amt;
    [SerializeField] private float attackDamge;
    [SerializeField] private float knockBack;
    private NewBehaviourScript pc; 
    
    private bool GotInput; 
    [SerializeField] private bool IsAttack; 
    private int AttackNumber; 
    private int MaxAttackNumber = 2; 
    private float AttackTran = 0.5f; 
    private float LastInputTime = Mathf.NegativeInfinity;
    private float[] AttackDetail = new float[3]; 
    [SerializeField] private LayerMask WhatIsDamgeEnable; 
    
    
    [Header("Hitbox")]
    [SerializeField] private Transform AttackHitBox1; 
    [SerializeField] private float AtackRadius1; 
    [SerializeField] private Transform AttackHitBox2;
    [SerializeField] private float AtackRadius2;
    
    [Header("Enchance")]
    [SerializeField] private int maxStack;
    private int curStack = 0;
    private bool isEnchanceAttack = false;
    [SerializeField] private GameObject enchanceEffect;
    [SerializeField] private Transform AmiyaCut;
    [SerializeField] private float AmiyaCutRadius;
    [SerializeField] private GameObject cutEffect;
    private bool isEnchanceAttackActive = false;
    private Coroutine enchaneAttackCoroutine;
    private bool isEnchanceAttackGravityActive = false;
    private Coroutine enchaneAttackGravityCoroutine;

    [Header("Burst")] 
    [SerializeField] private float burstBonusDamge;
    private bool isBurst = false;
    [SerializeField] private float burstCD;
    [SerializeField] private float burstTime;
    [SerializeField] private GameObject burstAnimation;
    private float lastBurstTime = -100.0f;
    private Echo echo;
    private float lastStack = -100f;
    private float gainStack = 1.0f;
    [SerializeField] private float burstBaseDamage;

    [Header("Skill")]
    [SerializeField] private float skillCD;
    private bool isSkill;
    private bool isSkillBurstFirst = false;
    private bool isSkillBurstSecond = false;
    private float lastSkillTime = -100.0f;
    private float lastSkill1BTime = -100.0f;
    [SerializeField] private float skill1BCD;
    [SerializeField] private float skillBusrtChange;
    [SerializeField] private Transform skillTransform;
    [SerializeField] private float skillRadius;
    [SerializeField] private float skillDamage;
    [SerializeField] private float skillLastDamage;

    [Header("Stack")] 
    [SerializeField]private GameObject[] stackObject;
    [SerializeField]private GameObject stackObjectParent;
    
    private void Start()
    {
        amt = GetComponent<Animator>();
        pc = GameObject.Find("Player").GetComponent<NewBehaviourScript>();
        AttackNumber = 1;
        curStack = maxStack;
        echo = GameObject.Find("PlayerTrail").GetComponent<Echo>();
    }

    private void Update()
    {
        if (base.IsPickUp == false) return; 
        
        amt.SetBool("IsPickUp", base.IsPickUp);
        CheckCombatInput(); 
        Attack(); 
        ResetAttack(); 
        SubAttack();
        subBurst();
        BurstMode();
        subSkillBurst();
    }
    
    private void PickUp(Transform tf)
    {
        base.PickUp(tf); 
        stackObjectParent.SetActive(true);
        StackVisible();
    }

    private void Drop()
    {
        base.Drop();
        
        amt.SetBool("IsPickUp", base.IsPickUp); 
        stackObjectParent.SetActive(false);
    }

    private void StackVisible()
    {
        for (int i = 0; i < stackObject.Length; i++)
        {
            if (i < curStack)
            {
                stackObject[i].SetActive(true);
                continue;
            }
            stackObject[i].SetActive(false);
        }
    }
    
    private void CheckCombatInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GotInput = true;
            LastInputTime = Time.time;
            isEnchanceAttack = false;
        }

        if (Input.GetMouseButtonUp(1) && curStack > 0 && !isEnchanceAttack)
        {
            curStack -= 1;
            LastInputTime = Time.time;
            isEnchanceAttack = true;

            enchanceAttack();
            StackVisible();
        }
        
        if (Input.GetKey(KeyCode.Q) && Time.time >= (lastBurstTime + burstCD) && isBurst == false)
        {
            isBurst = true;
            lastBurstTime = Time.time;
            echo.EnableTrail();
            amt.SetBool("IsBurst", true);
            enBurstChange();
            burstDamage();
            lastStack = Time.time;
        }
        
        if (Input.GetKey(KeyCode.E) && !isSkill)
        {
            Skill();
        }
    }

    private void Attack()
    {
        if(GotInput && !pc.GetIsDash() && !pc.GetIsWallSlide()) 
        {
            if (!IsAttack && !isEnchanceAttack) 
            {
                GotInput = false;
                IsAttack = true;
                
                if (AttackNumber == 1)
                {
                    amt.SetBool("IsAttack", IsAttack);
                    amt.SetBool("Attack_1", true);
                    
                    AttackNumber += 1;
                    if (AttackNumber > MaxAttackNumber)
                    {
                        AttackNumber = 1;
                    }
                    
                    Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AttackHitBox1.position, AtackRadius1, WhatIsDamgeEnable);

                    if (isBurst)
                    {
                        AttackDetail[0] = attackDamge + burstBonusDamge;
                    }
                    else
                    {
                        AttackDetail[0] = attackDamge;
                    }
                    
                    AttackDetail[1] = GameObject.Find("Player").transform.position.x; 
                    AttackDetail[2] = knockBack;
        
                    foreach(Collider2D Colider in DetectObject)
                    {
                        Colider.transform.SendMessage("Damage", AttackDetail);
                    }
                    
                }
                else if (AttackNumber == 2)
                {
                    amt.SetBool("IsAttack", IsAttack);
                    amt.SetBool("Attack_2", true);
                    
                    AttackNumber += 1;
                    if (AttackNumber > MaxAttackNumber)
                    {
                        AttackNumber = 1;
                    }
                    StackVisible();
                    
                    Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AttackHitBox2.position, AtackRadius2, WhatIsDamgeEnable);

                    if (isBurst)
                    {
                        AttackDetail[0] = attackDamge + burstBonusDamge;
                    }
                    else
                    {
                        AttackDetail[0] = attackDamge;
                    }
                    
                    AttackDetail[1] = GameObject.Find("Player").transform.position.x; 
                    AttackDetail[2] = knockBack;

                    curStack += 1;
                    if (curStack > maxStack)
                    {
                        curStack = maxStack;
                    }
        
                    foreach(Collider2D Colider in DetectObject)
                    {
                        Colider.transform.SendMessage("Damage", AttackDetail);
                    }
                }
            }
        }
    }

    private void enchanceAttack()
    {
        if (isEnchanceAttack && !IsAttack)
        {
            pc.GetRb().gravityScale = 0.0f;
            pc.GetRb().velocityY = 0.0f;
            pc.GetRb().velocityX = 0.0f;
            pc.DisableCanRun();
            pc.DisableInput();
            
            IsAttack = true;
            Instantiate(cutEffect, pc.transform);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(AmiyaCut.position, AmiyaCutRadius, WhatIsDamgeEnable);
            List<GameObject> Enemy = new List<GameObject>();

            foreach (Collider2D collider in colliders)
            {
                Enemy.Add(collider.gameObject);
            }
        
            Enemy.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position).CompareTo(Vector2.Distance(transform.position, b.transform.position)));

            if (Enemy.Count != 0)
            {
                Vector3 firstEnemyPosition = Enemy[0].transform.position;
                if (pc.transform.position.x < firstEnemyPosition.x && pc.GetFlipDirect() < 0.0f)
                {
                    pc.GetFlipping();
                }
                else if (pc.transform.position.x > firstEnemyPosition.x && pc.GetFlipDirect() > 0.0f)
                {
                    pc.GetFlipping();
                }
                
                GameObject instance = (GameObject)Instantiate(enchanceEffect, firstEnemyPosition, pc.transform.rotation);
            }
            else if (Enemy.Count == 0)
            {
                float randomX = 0.0f;
                if (pc.GetFlipDirect() > 0.0f)
                {
                    randomX = transform.position.x + 5.0f;
                }
                else if(pc.GetFlipDirect() < 0.0f)
                {
                    randomX = transform.position.x - 4.0f;
                }
            
                float PositionY = transform.position.y;
                Vector3 EnemyPosition = new Vector3(randomX, PositionY, transform.position.z);
                    
                GameObject instance = (GameObject)Instantiate(enchanceEffect, EnemyPosition, pc.transform.rotation);
            }

            subEnchaneAttack();
            subEnchaneAttackGravity();
        }
    }
    
    private void subEnchaneAttack()
    {
        if (isEnchanceAttackActive)
        {
            StopCoroutine(enchaneAttackCoroutine);
        }
        
        enchaneAttackCoroutine = StartCoroutine(ActivateAfterDelay(0.2f));
    }

    private IEnumerator ActivateAfterDelay(float delay)
    {
        isEnchanceAttackActive = true;

        yield return new WaitForSeconds(delay);

        if (isEnchanceAttack)
        {
            isEnchanceAttack = false;
            IsAttack = false;
            GameObject.Find("Player").GetComponent<Player_Weapion_Manager>().SubAttackDisable();
            pc.EnableInput();
        }

        isEnchanceAttackActive = false;
    }
    
    private void subEnchaneAttackGravity()
    {
        if (isEnchanceAttackGravityActive)
        {
            StopCoroutine(enchaneAttackGravityCoroutine);
        }
        
        enchaneAttackGravityCoroutine = StartCoroutine(ActivateAfterDelayGravity(0.3f));
    }
    private void subEnchaneAttackGravity(float x)
    {
        if (isEnchanceAttackGravityActive)
        {
            StopCoroutine(enchaneAttackGravityCoroutine);
        }
        
        enchaneAttackGravityCoroutine = StartCoroutine(ActivateAfterDelayGravity(x));
    }

    private IEnumerator ActivateAfterDelayGravity(float delay)
    {
        isEnchanceAttackGravityActive = true;

        yield return new WaitForSeconds(delay);
        
        pc.GetRb().gravityScale = 5.0f;
        pc.EnableCanRun();

        isEnchanceAttackGravityActive = false;
    }

    private void EndAttack()
    {
        IsAttack = false;
        amt.SetBool("Attack_1", false);
        amt.SetBool("Attack_2", false);
        amt.SetBool("IsAttack", false);
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

    private void burstDamage()
    {
        Instantiate(cutEffect, pc.transform);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(AmiyaCut.position, AmiyaCutRadius + 2.0f, WhatIsDamgeEnable);
        List<GameObject> Enemy = new List<GameObject>();

        foreach (Collider2D collider in colliders)
        {
            Enemy.Add(collider.gameObject);
        }
        
        Enemy.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position).CompareTo(Vector2.Distance(transform.position, b.transform.position)));

        if (Enemy.Count != 0)
        {
            Vector3 firstEnemyPosition = Enemy[0].transform.position;
            if (pc.transform.position.x < firstEnemyPosition.x && pc.GetFlipDirect() < 0.0f)
            {
                pc.GetFlipping();
            }
            else if (pc.transform.position.x > firstEnemyPosition.x && pc.GetFlipDirect() > 0.0f)
            {
                pc.GetFlipping();
            }
                
            GameObject instance = (GameObject)Instantiate(burstAnimation, firstEnemyPosition, pc.transform.rotation);
        }
        else if (Enemy.Count == 0)
        {
            
            float randomX = 0.0f;
            if (pc.GetFlipDirect() > 0.0f)
            {
                randomX = transform.position.x + 5.0f;
            }
            else if(pc.GetFlipDirect() < 0.0f)
            {
                randomX = transform.position.x - 4.0f;
            }
            
            float PositionY = transform.position.y;
            Vector3 EnemyPosition = new Vector3(randomX, PositionY, transform.position.z);
                    
            GameObject instance = (GameObject)Instantiate(burstAnimation, EnemyPosition, pc.transform.rotation);
        }
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
        maxStack = 5;
        curStack = maxStack;
        StackVisible();
        isSkillBurstFirst = true;
        lastSkill1BTime = -100f;
    }
    private void deBurstChange()
    {
        maxStack = 3;
        if(curStack > maxStack)
            curStack = maxStack;
        amt.SetBool("IsBurst", false);
        isSkillBurstFirst = false;
        lastSkillTime = -100f;
    }

    private void BurstMode()
    {
        if (isBurst)
        {
            if (Time.time > lastStack + gainStack)
            {
                lastStack = Time.time;
                curStack += 1;
                if (curStack > maxStack)
                {
                    curStack = maxStack;
                }
                StackVisible();
            }
        }
    }

    private void Skill()
    {
        if (!isBurst && Time.time > lastSkillTime + skillCD)
        {
            isSkill = true;
            lastSkillTime = Time.time;
            amt.SetBool("IsSkill1", isSkill);
            
            curStack += 1;
            if (curStack > maxStack)
            {
                curStack = maxStack;
            }
            StackVisible();
            
            Collider2D[] DetectObject = Physics2D.OverlapCircleAll(skillTransform.position, skillRadius, WhatIsDamgeEnable);
            
            AttackDetail[0] = skillDamage;
            AttackDetail[1] = GameObject.Find("Player").transform.position.x; 
            AttackDetail[2] = knockBack;
        
            foreach(Collider2D Colider in DetectObject)
            {
                Colider.transform.SendMessage("Damage", AttackDetail);
            }
        }
        else if (isBurst)
        {
            if (isSkillBurstFirst && !isSkillBurstSecond && Time.time > lastSkill1BTime + skill1BCD)
            {
                lastSkill1BTime = Time.time;
                isSkill = true;
                amt.SetBool("IsSkill1B", true);
                isSkillBurstFirst = false;
                isSkillBurstSecond = true;
                
                curStack += 2;
                if (curStack > maxStack)
                {
                    curStack = maxStack;
                }
                StackVisible();
                
                Collider2D[] DetectObject = Physics2D.OverlapCircleAll(skillTransform.position, skillRadius, WhatIsDamgeEnable);
                
                AttackDetail[0] = skillDamage + burstBonusDamge;
                AttackDetail[1] = GameObject.Find("Player").transform.position.x; 
                AttackDetail[2] = knockBack;
        
                foreach(Collider2D Colider in DetectObject)
                {
                    Colider.transform.SendMessage("Damage", AttackDetail);
                    Colider.transform.SendMessage("Damage", AttackDetail);
                }
            }
            else if(!isSkillBurstFirst && isSkillBurstSecond && Time.time < lastSkill1BTime + skillBusrtChange)
            {
                lastSkill1BTime = Time.time;
                isSkill = true;
                amt.SetBool("IsSkill2B", true);
                isSkillBurstSecond = false;
                
                curStack += 2;
                if (curStack > maxStack)
                {
                    curStack = maxStack;
                }
                StackVisible();
                
                Collider2D[] DetectObject = Physics2D.OverlapCircleAll(skillTransform.position, skillRadius, WhatIsDamgeEnable);
                
                AttackDetail[0] = skillDamage + burstBonusDamge;
                AttackDetail[1] = GameObject.Find("Player").transform.position.x; 
                AttackDetail[2] = knockBack;
        
                foreach(Collider2D Colider in DetectObject)
                {
                    Colider.transform.SendMessage("Damage", AttackDetail);
                    Colider.transform.SendMessage("Damage", AttackDetail);
                }
            }
            else if(!isSkillBurstFirst && !isSkillBurstSecond && Time.time < lastSkill1BTime + skillBusrtChange)
            {
                lastSkill1BTime = Time.time;
                isSkill = true;
                amt.SetBool("IsSkill3B", true);
                isSkillBurstFirst = true;
                
                curStack += 3;
                if (curStack > maxStack)
                {
                    curStack = maxStack;
                }
                StackVisible();
                
                Collider2D[] DetectObject = Physics2D.OverlapCircleAll(skillTransform.position, skillRadius, WhatIsDamgeEnable);
                
                AttackDetail[0] = skillLastDamage + burstBonusDamge;
                AttackDetail[1] = GameObject.Find("Player").transform.position.x; 
                AttackDetail[2] = knockBack;
        
                foreach(Collider2D Colider in DetectObject)
                {
                    Colider.transform.SendMessage("Damage", AttackDetail);
                }
            }
        }
    }

    private void subSkillBurst()
    {
        if (isBurst)
        {
            if (Time.time > lastSkill1BTime + skillCD)
            {
                isSkillBurstFirst = true;
            }
        }
    }

    private void EndSkill()
    {
        isSkill = false;
        amt.SetBool("IsSkill1", isSkill);
        amt.SetBool("IsSkill1B", false);
        amt.SetBool("IsSkill2B", false);
        amt.SetBool("IsSkill3B", false);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(AttackHitBox1.position, AtackRadius1);
        Gizmos.DrawWireSphere(AttackHitBox2.position, AtackRadius2);
        Gizmos.DrawWireSphere(AmiyaCut.position, AmiyaCutRadius);
        Gizmos.DrawWireSphere(skillTransform.position, skillRadius);
    }

    private void SubAttack()
    {
        if (IsAttack)
        {
            GameObject.Find("Player").GetComponent<Player_Weapion_Manager>().SubAttackEnable();
        }
    }

    public float GetAttackDamage()
    {
        return attackDamge;
    }
    public float GetKnockBack()
    {
        return knockBack;
    }

    public float GetAttackDamageBonus()
    {
        return burstBonusDamge;
    }

    public bool GetIsBurst()
    {
        return isBurst;
    }

    public float getBurstBaseDamage()
    {
        return burstBaseDamage;
    }
}
