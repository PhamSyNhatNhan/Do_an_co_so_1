using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool isMainMenu = true;
    [SerializeField] private GameObject menuScene;
    [SerializeField] private GameObject uiScene;
    [SerializeField] private GameObject pauseScene;
    
    private void Start()
    {
        Time.timeScale = 0;
        
        menuScene.SetActive(true);
        uiScene.SetActive(false);
        pauseScene.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isMainMenu)
            {
                Time.timeScale = 0;
                
                menuScene.SetActive(false);
                uiScene.SetActive(false);
                pauseScene.SetActive(true);
            }
        }
    }

    public void PlayGame()
    {
        Debug.Log("Game");
        if (isMainMenu)
        {
            Time.timeScale = 1;
            isMainMenu = false;
            
            menuScene.SetActive(false);
            uiScene.SetActive(true);
            pauseScene.SetActive(false);
        }
        else if(!isMainMenu)
        {
            Time.timeScale = 0;
            
            menuScene.SetActive(false);
            uiScene.SetActive(true);
            pauseScene.SetActive(false);
        }
    }
    public void QuitGame()
    {
        if (isMainMenu)
        {
            Application.Quit();
        }
        else if(!isMainMenu)
        {
            Time.timeScale = 0;
            isMainMenu = true;
            
            menuScene.SetActive(true);
            uiScene.SetActive(false);
            pauseScene.SetActive(false);
        }
    }

    public void test()
    {
        Debug.Log("Test");
    }
}
