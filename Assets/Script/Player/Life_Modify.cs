using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class Life_Modify : MonoBehaviour
{
    private Player_Stat ps;
    public Image[] life;
    
    void Start()
    {
        ps = GetComponent<Player_Stat>();
    }
    
    public void ModifyLife()
    {
        int lifeCount = (int)(ps.CurentHealth);
        
        for (int i = 0; i < 5; i++)
        {
            if (i < lifeCount)
            {
                life[i].enabled = true;
                continue;
            }
            life[i].enabled = false;
        }
    }
}
