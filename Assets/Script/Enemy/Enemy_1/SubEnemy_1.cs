using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubEnemy_1 : MonoBehaviour
{
    private Enemy_1_Controller em;
    [SerializeField]private GameObject weapon;
    private bool isDrop = false;
    
    void Start()
    {
        em = GetComponent<Enemy_1_Controller>();
    }
    
    void Update()
    {
        if (em.GetCurrentHealth() <= 0f && !isDrop)
        {
            isDrop = true;
            Vector2 pos = new Vector2(transform.position.x, transform.position.y + 5.0f);
            GameObject instance = (GameObject)Instantiate(weapon, pos, Quaternion.identity);
        }
    }
}
