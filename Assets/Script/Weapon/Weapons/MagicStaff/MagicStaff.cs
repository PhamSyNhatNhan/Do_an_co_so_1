using System.Collections;
using UnityEngine;

public class MagicStaff : SubWeapon
{
    [Header("Details")]
    private Animator amt;
    [SerializeField] private float attackDamage;
    [SerializeField] private float knockBack;
    private NewBehaviourScript pc; // player controller moi

    private bool GotInput; // co dau vao khong
    private bool CanAttack = true; // co the tan cong hay khong
    private bool IsAttack;
    private float LastInputTime = Mathf.NegativeInfinity;
    private float[] AttackDetail = new float[3];
    [SerializeField] private LayerMask WhatIsDamageEnable;
    private float rotationAngle;
    [SerializeField] private float delayTime;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float speed;

    private void Start()
    {
        amt = GetComponent<Animator>();
        pc = GameObject.Find("Player").GetComponent<NewBehaviourScript>();
    }

    private void Update()
    {

        if (base.IsPickUp == false) return;
        amt.SetBool("CanAttack", CanAttack);
        amt.SetBool("IsPickUp", base.IsPickUp);
        CheckCombatInput();
    }
    
    private void PickUp(Transform tf)
    {
        base.PickUp(tf); 
    }

    private void Drop()
    {
        base.Drop();
        
        amt.SetBool("IsPickUp", base.IsPickUp); 
    }

    private void CheckCombatInput()
    {
        if (Input.GetMouseButtonDown(0) && CanAttack)
        {
            GotInput = true;
            LastInputTime = Time.time;
            Attack();
        }
    }

    private void Attack()
    {
        if (GotInput && !pc.GetIsDash() && !pc.GetIsWallSlide())
        {
            GotInput = false;
            IsAttack = true;
            CanAttack = false; // Không thể tấn công trong thời gian CanAttack = false
            amt.SetBool("IsAttack", true);
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            
            if (pc.transform.position.x < mousePosition.x && pc.GetFlipDirect() < 0.0f)
            {
                pc.GetFlipping();
            }
            else if (pc.transform.position.x > mousePosition.x && pc.GetFlipDirect() > 0.0f)
            {
                pc.GetFlipping();
            }

            Vector2 shootDirection = (mousePosition - transform.position).normalized;

            float angle = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x) * Mathf.Rad2Deg;
            rotationAngle = angle;

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            projectileScript.Initialize(shootDirection, speed, attackDamage, knockBack, WhatIsDamageEnable);

            transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

            StartCoroutine(ResetCanAttack(delayTime)); // Đặt lại CanAttack sau x giây
        }
    }

    private IEnumerator ResetCanAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        CanAttack = true;// Đặt lại CanAttack thành true sau khoảng thời gian delay
        IsAttack = false;
        amt.SetBool("IsAttack", false);
    }

    private void EndAttack()
    {
        amt.SetBool("IsAttack", false);
    }

    public bool getIsAttack()
    {
        return IsAttack;
    }
}
