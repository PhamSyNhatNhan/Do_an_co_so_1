using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword_1 : SubWeapon
{
    [Header("Details")]
    private Animator amt;
    [SerializeField] private float attackDamge;
    [SerializeField] private float knockBack;
    private NewBehaviourScript pc; // player controller moi
    
    private bool GotInput; // co dau vao khong
    [SerializeField] private bool IsAttack; // co dang tan cong khong
    private int AttackNumber; // vi tri don danh hien tai
    private int MaxAttackNumber = 2; // so don danh trong combo
    private float AttackTran = 0.5f; // gian cach toi da giua cac don danh
    private float LastInputTime = Mathf.NegativeInfinity;
    private float[] AttackDetail = new float[3]; // attack damage, vi tri theo truc x, gia tri cua knock back
    [SerializeField] private LayerMask WhatIsDamgeEnable; // thêm layer mask damgeable vào phần này
    
    
    [Header("Hitbox")]
    [SerializeField] private Transform AttackHitBox1; // vị trí của hit box của đòn 1
    [SerializeField] private float AtackRadius1; // độ lớn của hit box
    [SerializeField] private Transform AttackHitBox2;
    [SerializeField] private float AtackRadius2;
    
    

    private void Start()
    {
        amt = GetComponent<Animator>();
        pc = GameObject.Find("Player").GetComponent<NewBehaviourScript>();
        AttackNumber = 1;
    }

    private void Update()
    {
        if (base.IsPickUp == false) return; // nếu vũ khi không được cầm thì không chạy các update
        
        amt.SetBool("IsPickUp", base.IsPickUp);
        CheckCombatInput(); // kiểm tra xem có giá trị đầu vào cho input không
        Attack(); // các đoàn trong chuỗi combo
        ResetAttack(); // sau đủ thời gian thì reset về đòn 1 nếu có nhiều hơn 1 đòn trong chuỗi, không thì bỏ qua
        SubAttack();
    }
    
    private void PickUp(Transform tf)
    {
        base.PickUp(tf); // chạy pickup của cha
    }

    private void Drop()
    {
        base.Drop();
        
        amt.SetBool("IsPickUp", base.IsPickUp); // chạy drop của cha và đặt lại animation về sprites mặc định
    }
    
    private void CheckCombatInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GotInput = true;
            LastInputTime = Time.time;
            // nếu có input thì đặt mốc thời gian cho cờ
        }
    }

    private void Attack()
    {
        if(GotInput && !pc.GetIsDash() && !pc.GetIsWallSlide()) 
        {
            if (!IsAttack) // nếu có input và đang không trong 1 đòn tấn công khác
            {
                GotInput = false;
                IsAttack = true;
                
                // các đòn trong combo
                if (AttackNumber == 1)
                {
                    amt.SetBool("IsAttack", IsAttack);
                    amt.SetBool("Attack_1", true);
                    
                    AttackNumber += 1;
                    if (AttackNumber > MaxAttackNumber)
                    {
                        AttackNumber = 1;
                    }
                    
                    // tìm kiếm đối tượng trong phạm vi của hit box
                    Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AttackHitBox1.position, AtackRadius1, WhatIsDamgeEnable);

                    AttackDetail[0] = attackDamge;
                    AttackDetail[1] = GameObject.Find("Player").transform.position.x; // lấy theo vị trí trục x của player
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
                    
                    Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AttackHitBox2.position, AtackRadius2, WhatIsDamgeEnable);

                    AttackDetail[0] = attackDamge;
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
        // được đặt vào cuối animation của đòn tấn công
        IsAttack = false;
        amt.SetBool("Attack_1", false);
        amt.SetBool("Attack_2", false);
        amt.SetBool("IsAttack", false);
        GameObject.Find("Player").GetComponent<Player_Weapion_Manager>().SubAttackDisable();
    }
    
    private void ResetAttack()
    {
        // sau đủ thời gian không tấn công sẽ reset về 1 hoặc nếu comb hiện tại vượt quá giới hạn
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
        // cái vòng tròn để kiểm tra hitbox
        Gizmos.DrawWireSphere(AttackHitBox1.position, AtackRadius1);
        Gizmos.DrawWireSphere(AttackHitBox2.position, AtackRadius2);
    }

    private void SubAttack()
    {
        if (IsAttack)
        {
            GameObject.Find("Player").GetComponent<Player_Weapion_Manager>().SubAttackEnable();
        }
    }
}

// tạo script mới thì nhớ kết thừa subweapon
// tạo đầy đủ các biến trong mục detailer
// tạo các hàm còn lại nếu chỉ có 1 đòn duy nhất thì bỏ qua hàm reset attack
// end attack sử dụng animation event để ở cuối animations
// animation nên làm theo animation của sword1 
// 