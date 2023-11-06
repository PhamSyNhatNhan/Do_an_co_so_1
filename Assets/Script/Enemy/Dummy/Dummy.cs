using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private float MaxHealth;
    private float CurHealth;
   

    [Header("KnockBack")]
    [SerializeField] private bool ApplyKnockBack;
    [SerializeField] private float KnockBackX, KnockBackY;
    private float KnockBackStart;
    [SerializeField] private float KnockBackDuration;
    private bool KnockBack;

    private Rigidbody2D rb;
    private Player_Controller pc;
    [Header("Hit")] [SerializeField] private GameObject HitParticle;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CurHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        CheckKnockback();
    }

    public void Damage(float[] atkDetails)
    {
        CurHealth -= atkDetails[0];

        Instantiate(HitParticle, transform);

        if (ApplyKnockBack && CurHealth > 0.0f)
        {
            float PlayerDirect = 0;
            if (transform.position.x > atkDetails[1]) PlayerDirect = 1;
            else if (transform.position.x < atkDetails[1]) PlayerDirect = -1;
            
            Knockback(PlayerDirect, atkDetails[2]);
        }
    }

    private void Knockback(float PlayerDirect, float knockforce)
    {
        KnockBack = true;
        KnockBackStart = Time.time;
        rb.velocity = new Vector2(KnockBackX * PlayerDirect * knockforce, KnockBackY);
    }

    private void CheckKnockback()
    {
        if(Time.time > KnockBackStart + KnockBackDuration && KnockBack) {
            KnockBack = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }
}
