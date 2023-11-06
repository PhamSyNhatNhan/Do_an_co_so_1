using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubWeapon : MonoBehaviour
{
    private Rigidbody2D rb ;
    private BoxCollider2D bc;
    
    [SerializeField] private float throwForceX = 0.0f;
    [SerializeField] private float throwForceY = 5.0f;
    private bool isPickUp;

    public bool IsPickUp
    {
        get => isPickUp;
        set => isPickUp = value;
    }

    public void PickUp(Transform tf)
    {
        if(isPickUp) return;
        
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        Destroy(rb);
        Destroy(bc);
        
        transform.parent = tf;
        
        rb.transform.rotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        GameObject.Find("Player").GetComponent<Player_Weapion_Manager>().SubPickUp();
        
        isPickUp = true;
    }

    public void Drop()
    {
        if(!isPickUp) return;
        
        bc = gameObject.AddComponent<BoxCollider2D>();
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.transform.rotation = Quaternion.identity;
        
        Vector2 throwForce = new Vector2(throwForceX, throwForceY);
        rb.AddForce(throwForce, ForceMode2D.Impulse);
        
        transform.parent = null;
        isPickUp = false;
    }
}
