using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubEnemy_1 : MonoBehaviour
{
    private Enemy_1_Controller em;
    [SerializeField]private GameObject weapon;
    private bool isDrop = false;
    [SerializeField] private GameObject minion;
    private float lastSpawnTime = -100f;
    private bool isFrist = false;
    
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

        spawnMinion();
        firstHalf();
    }

    private void spawnMinion()
    {
        if (em.GetCurrentHealth() <= 70f && Time.time > (lastSpawnTime + 2.5f))
        {
            lastSpawnTime = Time.time;
            float randomX = transform.position.x + Random.Range(-2f, 2f);
            
            Vector3 randomPosition = new Vector3(randomX, transform.position.y, transform.position.z);
            
            GameObject instance = (GameObject)Instantiate(minion, randomPosition, Quaternion.identity);

            if (em.GetCurrentHealth() <= 40f)
            {
                randomX = transform.position.x + Random.Range(-2f, 2f);
                randomPosition = new Vector3(randomX, transform.position.y, transform.position.z);
                instance = (GameObject)Instantiate(minion, randomPosition, Quaternion.identity);
            }
        }
    }

    private void firstHalf()
    {
        if (em.GetCurrentHealth() <= 50f && !isFrist)
        {
            isFrist = true;
            Debug.Log(isFrist);
            for (int i = 0; i < 6; i++)
            {
                float randomX = transform.position.x + Random.Range(-2f, 2f);
            
                Vector3 randomPosition = new Vector3(randomX, transform.position.y, transform.position.z);
            
                GameObject instance = (GameObject)Instantiate(minion, randomPosition, Quaternion.identity);
            }
        }
    }
}
