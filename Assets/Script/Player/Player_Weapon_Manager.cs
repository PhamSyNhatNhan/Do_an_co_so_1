using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Weapion_Manager : MonoBehaviour
{
    [SerializeField] private LayerMask weaponLayer;
    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private Transform weaponTransform;
    private bool isFind = false;
    private bool isHoldWeapon = false;
    
    private void Update()
    {
        CheckInPut();
    }

    private void FixedUpdate()
    {
        FindAndPickWeapon();
    }

    private void CheckInPut()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFind = true;
        }
    }

    private void FindAndPickWeapon()
    {
        if(!isFind) return;
        isFind = false;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, weaponLayer);
        List<GameObject> weaponObjects = new List<GameObject>();

        foreach (Collider2D collider in colliders)
        {
            weaponObjects.Add(collider.gameObject);
        }
        
        weaponObjects.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position).CompareTo(Vector2.Distance(transform.position, b.transform.position)));

        if (weaponObjects.Count != 0)
        {
            if (isHoldWeapon)
            {
                try
                {
                    GameObject weaponManager = GameObject.Find("WeaponManager");
                    
                    Transform weaponChildTransform = weaponManager.transform.GetChild(0);

                    if (weaponChildTransform != null)
                    {
                        GameObject weaponChildObject = weaponChildTransform.gameObject;

                        weaponChildObject.SendMessage("Drop");
                    }
                    
                    isHoldWeapon = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            weaponObjects[0].SendMessage("PickUp", weaponTransform.transform);
        }
    }

    public void SubPickUp()
    {
        isHoldWeapon = true;
    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(weaponTransform.position, detectionRadius);
    }
}
