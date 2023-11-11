using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Weapion_Manager : MonoBehaviour
{
    [SerializeField] private LayerMask weaponLayer;
    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private GameObject weaponPoision;
    [SerializeField] private float weaponPoisionRadius;
    private bool isFind = false;
    private bool isHoldWeapon = false;
    private Coroutine fadeCoroutine;
    
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
            
            SpriteRenderer weaponPoisionRenderer = weaponPoision.GetComponent<SpriteRenderer>();
            
            SpriteRenderer pickedWeaponRenderer = weaponObjects[0].GetComponent<SpriteRenderer>();
            
            weaponPoisionRenderer.sprite = pickedWeaponRenderer.sprite;
            
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
            weaponPoisionRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public void SubAttackEnable()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        SpriteRenderer weaponPoisionRenderer = weaponPoision.GetComponent<SpriteRenderer>();
        weaponPoisionRenderer.color = new Color(1f, 1f, 1f, 0f);
    }
    public void SubAttackDisable()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    
        fadeCoroutine = StartCoroutine(FadeIn(2.5f));
    }

    private IEnumerator FadeIn(float duration)
    {
        SpriteRenderer weaponPoisionRenderer = weaponPoision.GetComponent<SpriteRenderer>();
        float startAlpha = 0.0f; 
        float currentTime = 0f;
        float finalAlpha = 1f;
        
        while (currentTime < 2f)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, finalAlpha, (currentTime - 2f) / 0.5f); 

            Color newColor = weaponPoisionRenderer.color;
            newColor.a = alpha;
            weaponPoisionRenderer.color = newColor;

            yield return null;
        }
        
        fadeCoroutine = null;
    }

    public void SubPickUp()
    {
        isHoldWeapon = true;
    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(weaponTransform.position, detectionRadius);
        Gizmos.DrawSphere(weaponPoision.transform.position, weaponPoisionRadius);
    }
}
