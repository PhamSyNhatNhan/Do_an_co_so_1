using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fish : SubWeapon
{
    [Header("Details")]
    private Animator amt;
    [SerializeField] private float attackDamge;
    [SerializeField] private float knockBack;
    private NewBehaviourScript pc; // player controller moi

    private bool GotInput; // co dau vao khong
    private bool IsAttack; // co dang tan cong khong
    private int AttackNumber; // vi tri don danh hien tai
    private int MaxAttackNumber = 2; // so don danh trong combo
    private float AttackTran = 0.5f; // gian cach toi da giua cac don danh
    private float LastInputTime = Mathf.NegativeInfinity;
    private float[] AttackDetail = new float[3]; // attack damage, vi tri theo truc x, gia tri cua knock back
    [SerializeField] private LayerMask WhatIsDamgeEnable; // thêm layer mask damgeable vào phần này

    private bool Charging; // Biến để theo dõi trạng thái tụ lực
    private float ChargeTime; // Thời gian bạn đã tụ lực
    private float MinChargeTime = 1f; // Thời gian tối thiểu cần để tụ lực

    [Header("Hitbox")]
    [SerializeField] private Transform AttackHitBox; // vị trí của hit box của đòn 1
    [SerializeField] private float AtackRadius; // độ lớn của hit box



    private void Start()
    {
        amt = GetComponent<Animator>();
        pc = GameObject.Find("Player").GetComponent<NewBehaviourScript>();
        AttackNumber = 1;
    }

    private void Update()
    {
        if (base.IsPickUp == false) return;

        amt.SetBool("IsPickUp", base.IsPickUp);
        Charge();
        CheckCombatInput();
        ResetAttack();
        SubAttack();
    }

    private void Charge()
    {
        if (Charging)
        {
            ChargeTime += Time.deltaTime;
            if (ChargeTime < MinChargeTime)
            {
                // Bắt đầu animation tụ lực
                amt.SetBool("Charging", true);
            }
            else if (ChargeTime >= MinChargeTime)
            {
                // Bắt đầu animation tụ đầy
                amt.SetBool("FullyCharged", true);
            }
        }
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
        if (Input.GetMouseButtonDown(0))
        {
            GotInput = true;
            LastInputTime = Time.time;
            Charging = true; // Bắt đầu tụ lực khi ấn chuột
            ChargeTime = 0f; // Đặt thời gian tụ lực về 0
        }

        if (Input.GetMouseButtonUp(0) && Charging)
        {
            if (ChargeTime >= MinChargeTime)
            {
                // Khi tụ lực đủ thời gian, bắt đầu tấn công
                Attack();
            }
            Charging = false; // Ngừng tụ lực khi thả chuột
            amt.SetBool("Charging", false);
            amt.SetBool("FullyCharged", false); // Kết thúc animation tụ lực
        }
    }

    private void Attack()
    {
        if (GotInput && !pc.GetIsDash() && !pc.GetIsWallSlide())
        {
            if (!IsAttack) // nếu có input và đang không trong 1 đòn tấn công khác
            {
                GotInput = false;
                IsAttack = true;

                // các đòn trong combo
                if (AttackNumber == 1)
                {
                    amt.SetBool("IsAttack", IsAttack);

                    AttackNumber += 1;
                    if (AttackNumber > MaxAttackNumber)
                    {
                        AttackNumber = 1;
                    }

                    // tìm kiếm đối tượng trong phạm vi của hit box
                    Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AttackHitBox.position, AtackRadius, WhatIsDamgeEnable);

                    AttackDetail[0] = attackDamge;
                    AttackDetail[1] = GameObject.Find("Player").transform.position.x; // lấy theo vị trí trục x của player
                    AttackDetail[2] = knockBack;

                    foreach (Collider2D Colider in DetectObject)
                    {
                        Colider.transform.SendMessage("Damage", AttackDetail);
                    }
                }
                else if (AttackNumber == 2)
                {
                    amt.SetBool("IsAttack", IsAttack);

                    AttackNumber += 1;
                    if (attackDamge > MaxAttackNumber)
                    {
                        AttackNumber = 1;
                    }

                    Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AttackHitBox.position, AtackRadius, WhatIsDamgeEnable);

                    AttackDetail[0] = attackDamge;
                    AttackDetail[1] = GameObject.Find("Player").transform.position.x;
                    AttackDetail[2] = knockBack;

                    foreach (Collider2D Colider in DetectObject)
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
        amt.SetBool("IsAttack", IsAttack);
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
    
    private void SubAttack()
    {
        if (IsAttack)
        {
            GameObject.Find("Player").GetComponent<Player_Weapion_Manager>().SubAttackEnable();
        }
    }

    private void OnDrawGizmos()
    {
        // cái vòng tròn để kiểm tra hitbox
        Gizmos.DrawWireSphere(AttackHitBox.position, AtackRadius);
    }
}

// tạo script mới thì nhớ kết thừa subweapon
// tạo đầy đủ các biến trong mục detailer
// tạo các hàm còn lại nếu chỉ có 1 đòn duy nhất thì bỏ qua hàm reset attack
// end attack sử dụng animation event để ở cuối animations
// animation nên làm theo animation của sword1 
// 