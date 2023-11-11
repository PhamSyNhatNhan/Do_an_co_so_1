using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private MagicStaff magicStaff;
    private Animator amt;
    private Vector2 direction;
    private float speed;
    private float damage;
    private float knockBack;
    private LayerMask damageableLayer;
    private float[] AttackDetail = new float[3];

    private bool hasDealtDamage = false;
    private bool isAttack;
    private bool isClone = false;

    [Header("Hitbox")]
    [SerializeField] private Transform AttackHitBox; // vị trí của hit box của đòn 1
    [SerializeField] private float AttackRadius; // độ lớn của hit box


    private void Start()
    {
        amt = GetComponent<Animator>();
    }
    public void Initialize(Vector2 dir, float spd, float dmg, float kback, LayerMask dmgLayer)
    {
        direction = dir;
        speed = spd;
        damage = dmg;
        knockBack = kback;
        damageableLayer = dmgLayer;

        isClone = true;

        StartCoroutine(DestroyAfterDelay(2.5f)); // Thời gian tồn tại của đạn

        RotateSprite();
    }

    private void Update()
    {
        if (isClone)
        {
            Move();
            DealDamage();
            CheckIsAttack();
        }
    }

    private void RotateSprite()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Move()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }


    private void DealDamage()
    {
        
        Collider2D[] DetectObject = Physics2D.OverlapCircleAll(AttackHitBox.position, AttackRadius, damageableLayer);

        AttackDetail[0] = damage;
        AttackDetail[1] = transform.position.x;
        AttackDetail[2] = knockBack;

        foreach (Collider2D Colider in DetectObject)
        {
            if (hasDealtDamage == true)
            {
                break;
            }
            Colider.transform.SendMessage("Damage", AttackDetail);
            hasDealtDamage = true;
            amt.SetBool("DealtDamage", true);
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // cái vòng tròn để kiểm tra hitbox
        Gizmos.DrawWireSphere(AttackHitBox.position, AttackRadius);
    }

    private void CheckIsAttack()
    {
            magicStaff = GameObject.Find("MagicStaff").GetComponent<MagicStaff>();
            isAttack = magicStaff.getIsAttack();
            amt.SetBool("IsAttack", isAttack);
    }
}
