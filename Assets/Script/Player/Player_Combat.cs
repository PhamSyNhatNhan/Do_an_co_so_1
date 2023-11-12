using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Combat_ : MonoBehaviour
{
    private Player_Stat ps;
    private NewBehaviourScript pc;
    private Life_Modify lm;
    private float KnockbackStartTime = -100f;
    private bool Knockback = false;
    [SerializeField] private float KnockbackDuration;
    [SerializeField] private Vector2 KnockbackSpeed;
    [SerializeField] private Transform curCheckPoint;
    [SerializeField] private LayerMask checkPointLayer;
    [SerializeField] private float detectionRadius;
    [SerializeField] private GameObject checkPointBlink;
    
    void Start()
    {
        ps = GetComponent<Player_Stat>();
        pc = GetComponent<NewBehaviourScript>();
        lm = GetComponent<Life_Modify>();
    }
    
    void Update()
    {
        checkKnockback();
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            findCheckPoint();
        }
    }
    
    public void Damage(float[] atkDetails)
    {
         ps.CurentHealth -= atkDetails[0];
         lm.ModifyLife();

        if (ps.CurentHealth > 0.0f)
        {
            float PlayerDirect = 0;
            if (transform.position.x > atkDetails[1]) PlayerDirect = 1;
            else if (transform.position.x < atkDetails[1]) PlayerDirect = -1;
            
            KnockBack(PlayerDirect, atkDetails[2]);
        }
        else if (ps.CurentHealth <= 0.0f)
        {
            pc.DisableInput();
            pc.GetRb().velocity = new Vector2(0.0f, 0.0f);
            pc.GetRb().isKinematic = true;
            pc.GetCapsuleCollider().enabled = false;
            pc.GetAnimator().SetBool("Death", true);
        }
    }

    private void findCheckPoint()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, checkPointLayer);
        List<GameObject> checkPoint = new List<GameObject>();
        
        foreach (Collider2D collider in colliders)
        {
            checkPoint.Add(collider.gameObject);
        }
        checkPoint.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position).CompareTo(Vector2.Distance(transform.position, b.transform.position)));

        if (checkPoint.Count != 0)
        {
            ps.CurentHealth = ps.MaxHealth;
            Instantiate(checkPointBlink, transform);
            curCheckPoint = checkPoint[0].transform;
        }
    }

    private void endDeath()
    {
        pc.EnableInput();
        pc.GetRb().isKinematic = false;
        pc.GetCapsuleCollider().enabled = true;
        pc.GetAnimator().SetBool("Death", false);
        ps.CurentHealth = ps.MaxHealth;
        lm.ModifyLife();
        Vector3 respawn = new Vector3(curCheckPoint.position.x, curCheckPoint.position.y + 5.0f, 0.0f);
        pc.transform.position = respawn;
    }
    
    private void checkKnockback()
    {
        if (Time.time >= KnockbackStartTime + KnockbackDuration && Knockback)
        {
            Knockback = false;
            pc.GetRb().velocity = new Vector2(0.0f, pc.GetRb().velocity.y);
        }
    }
    
    public void KnockBack(float PlayerDirect, float knockbackforce)
    {
        if (knockbackforce == 0.0f) return;
        
        KnockbackStartTime = Time.time;
        Knockback = true;
        pc.GetRb().velocity = new Vector2(KnockbackSpeed.x * knockbackforce * PlayerDirect, KnockbackSpeed.y);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
