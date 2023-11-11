using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Echo : MonoBehaviour
{
    private bool CanSpawn = false;
    private NewBehaviourScript pc;
    private float timeSpawn;
    [SerializeField] private float startTimeSpawn;
    [SerializeField] private float destroyTimeSpawn;
    [SerializeField] private GameObject[] echo;
    
    private float timeSpawn2;
    [SerializeField] private float startTimeSpawn2;
    [SerializeField] private float destroyTimeSpawn2;
    [SerializeField] private GameObject[] echo2;

    void Start()
    {
        pc = GameObject.Find("Player").GetComponent<NewBehaviourScript>();
    }
    
    void Update()
    {
        if (timeSpawn <= 0 && CanSpawn)
        {
            int rand = Random.Range(0, echo.Length);
            float randomX = transform.position.x + Random.Range(-1f, 1f);
            float randomY = transform.position.y + Random.Range(-1f, 1f);
            Vector3 randomPosition = new Vector3(randomX, randomY, transform.position.z);
            
            GameObject instance = (GameObject)Instantiate(echo[rand], randomPosition, transform.rotation);
            Destroy(instance, destroyTimeSpawn);
            timeSpawn = startTimeSpawn;
        }
        else
        {
            timeSpawn -= Time.deltaTime;
        }
        
        
        if (timeSpawn2 <= 0 && CanSpawn && !(pc.GetRb().velocityX == 0.0f && pc.GetRb().velocityY == 0.0f))
        {
            int rand2 = Random.Range(0, echo2.Length);
            
            GameObject instance2 = (GameObject)Instantiate(echo2[rand2], transform.position, transform.rotation);
            Destroy(instance2, destroyTimeSpawn2);
            timeSpawn2 = startTimeSpawn2;
        }
        else
        {
            timeSpawn2 -= Time.deltaTime;
        }
    }

    public void EnableTrail()
    {
        CanSpawn = true;
    }

    public void DisableTrail()
    {
        CanSpawn = false;
    }
}
