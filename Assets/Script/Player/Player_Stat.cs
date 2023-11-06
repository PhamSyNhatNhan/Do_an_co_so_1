using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player_Stat : MonoBehaviour
{
    [Header("Move")] 
    [SerializeField] private float moveSpeed = 20.0f;

    [FormerlySerializedAs("JumpForce")]
    [Header("Jump")]
    [SerializeField] private float jumpForce = 15.0f;
    [SerializeField] private int amountJump = 2;
    [SerializeField] private float jumpvar;
    
    [FormerlySerializedAs("Airforce")]
    [Header("Air")]
    [SerializeField] private float airForce;
    [SerializeField] private float airDrag;

    [FormerlySerializedAs("DashTime")]
    [Header("Dash")]
    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCD;


    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = value;
    }

    public int AmountJump
    {
        get => amountJump;
        set => amountJump = value;
    }

    public float Jumpvar
    {
        get => jumpvar;
        set => jumpvar = value;
    }

    public float AirForce
    {
        get => airForce;
        set => airForce = value;
    }

    public float AirDrag
    {
        get => airDrag;
        set => airDrag = value;
    }

    public float DashTime
    {
        get => dashTime;
        set => dashTime = value;
    }

    public float DashSpeed
    {
        get => dashSpeed;
        set => dashSpeed = value;
    }

    public float DashCd
    {
        get => dashCD;
        set => dashCD = value;
    }
}