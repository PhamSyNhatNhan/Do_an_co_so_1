using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NewBehaviourScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private Player_Stat ps;
    private Animator amt;
    private bool CanInput = true;
    
    [Header("Movement")]
    private float InputDirect;
    private float FlipDirect = 1.0f;
    private bool IsRun;
    private bool CanRun = true;
    private bool CanFlip = true;
    
    [Header("Jump")]
    private bool CanJump;
    private bool IsJump;
    private int jumpleft;
    [SerializeField] GameObject DoubleJumpEffect;

    [Header("Ground check")]
    private bool IsGrounded;
    public Transform GroundCheck;
    public float GroundCheckRadius;
    public LayerMask WhatisGround;

    [Header("Wall check")]
    [SerializeField] private float WallCheckDistance;
    private bool IsTouchWall;
    public Transform WallCheck;
    private bool IsWallSlide;
    [SerializeField] private float WallSildeSpeed;

    [Header("Wall jump")]
    [SerializeField] private Vector2 WallJump;
    [SerializeField] private float WallJumpForce;
    private float VerticalDirect;

    [Header("Dash")]
    private float DashTimeLeft;
    private float LastDash = -100f;
    private bool IsDash;
    [SerializeField] GameObject DashEffect;
    [FormerlySerializedAs("AmountDash")] [SerializeField] private int amountDash = 2;
    [FormerlySerializedAs("DashLeft")] [SerializeField] private int dashLeft;
    private bool AirDash = false;
    private float LastDashMulti;
    private bool SupLastDash = false;
    private float SupLastDashTime;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        amt = GetComponent<Animator>();
        ps = GetComponent<Player_Stat>();
        jumpleft = ps.AmountJump;
        dashLeft = amountDash;
        WallJump.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCanJump();
        CheckInput();
        CheckFliping();
        AnimationControl();
        CheckWallSilde();
        SupDash();
        CheckDash();
    }
    private void FixedUpdate()
    {
        Movement();
        CheckSurrounding();
    }
    
    private void CheckSurrounding()
    {
        IsGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, WhatisGround);
        if (IsGrounded )
        {
            AirDash = false;
        }
        IsTouchWall = Physics2D.Raycast(WallCheck.position, transform.right, WallCheckDistance, WhatisGround);
    }
    
    private void CheckFliping()
    {
        if (IsDash) return;
        if (FlipDirect > 0.0f && InputDirect < 0)
        {
            Fliping();
        }
        else if(FlipDirect < 0.0f && InputDirect > 0)
        {
            Fliping();
        }

        if(rb.velocity.x != 0)
        {
            IsRun = true;
        }
        else
        {
            IsRun = false;
        }
    }

    
    
    private void CheckCanJump()
    {
        if ((IsGrounded && rb.velocity.y <= 0) || IsWallSlide)
        {
            jumpleft = ps.AmountJump;
            amt.SetBool("IsDoubleJump", false);
        }

        if(jumpleft <= 0)
        {
            CanJump = false;
            amt.SetBool("IsDoubleJump", true);
        }
        else
        {
            CanJump = true;
            amt.SetBool("IsDoubleJump", false);
        }
    }
    private void CheckWallSilde()
    {
        if(IsTouchWall && !IsGrounded && rb.velocity.y < 0.01f)
        {
            IsWallSlide = true;
        }
        else {
            IsWallSlide = false; 
        }
    }

    public void StartAni()
    {
        CanFlip = false;
    }
    public void EndAni()
    {
        CanFlip = true;
    }

    private void Fliping()
    {
        if(!IsWallSlide && CanFlip)
        {
            FlipDirect *= -1;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void CheckInput()
    {
        if(!CanInput) return;
        
        InputDirect = Input.GetAxisRaw("Horizontal");
        VerticalDirect = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * ps.Jumpvar);
        }

        if (Input.GetButtonDown("Dash"))
        {
            if (Time.time >= (LastDash + ps.DashCd) && !IsWallSlide && IsGrounded)
            {
                if(dashLeft <= 0 || LastDashMulti == LastDash)
                {
                    dashLeft = amountDash;
                }

                AttempToDash();
            }
            else if((Time.time >= (LastDash + ps.DashCd) && !IsWallSlide && !IsGrounded && !AirDash))
            {
                if (dashLeft <= 0)
                {
                    dashLeft = amountDash - 1;
                } 
                AirDash = true;
                AttempToDash();
            }  
        }
    }
    private void SupDash(){
        if (!SupLastDash) { return; }

        if(SupLastDashTime <= 0)
        {
            LastDash = LastDashMulti;
            SupLastDash = false;
        }
        else
        {
            SupLastDashTime -= Time.deltaTime;
        }
    }
    private void AttempToDash()
    {
        if(dashLeft == 1)
        {
            LastDash = Time.time;
            SupLastDash = false;
        }
        else if(dashLeft > 1)
        {
            SupLastDash = true;
            SupLastDashTime = ps.DashCd;
            LastDashMulti = Time.time;
        }

        IsDash = true;
        DashTimeLeft = ps.DashTime;
    }
    
    private void CheckDash()
    {
        if (IsDash)
        {
            if(DashTimeLeft > 0)
            {
                rb.velocity = new Vector2(ps.DashSpeed * FlipDirect, 0);
                amt.SetBool("IsDash", IsDash);
                if(DashTimeLeft == ps.DashTime)
                {
                    Instantiate(DashEffect, transform);
                    dashLeft--;
                }
                DashTimeLeft -= Time.deltaTime;
            }

            if(DashTimeLeft <= 0 || IsTouchWall)
            {
                IsDash = false;
                amt.SetBool("IsDash", IsDash);
            }
        }
    }

    private void Jump()
    {
        if (IsDash && IsGrounded) return;
        if(CanJump && !IsWallSlide)
        {
            IsDash = false;
            amt.SetBool("IsDash", IsDash);
            rb.velocity = new Vector2 (rb.velocity.x, ps.JumpForce);
            jumpleft -= 1;

            LastDash = -100f;
            AirDash = false;

            if (jumpleft == 0)
            {
                Instantiate(DoubleJumpEffect, transform);
            }
            
        }
        else if (IsWallSlide && InputDirect == 0 && CanJump)
        {
            IsWallSlide = false;
            jumpleft--;
            Vector2 ForceAdd_ = new Vector2(2f * -FlipDirect, 5f);
            rb.AddForce(ForceAdd_, ForceMode2D.Impulse);
        }
        
        else if((IsWallSlide || IsTouchWall) && InputDirect != 0 && CanJump)
        {
            if(FlipDirect != InputDirect)
            {
                IsWallSlide = false;
                jumpleft--;
                Vector2 ForceAdd_ = new Vector2(WallJumpForce * WallJump.x * InputDirect, WallJumpForce * WallJump.y);
                rb.AddForce(ForceAdd_, ForceMode2D.Impulse);
            }
        }
    }
    private void Movement()
    {
        if (IsDash) return;
        if (IsGrounded && CanRun)
        {
            rb.velocity = new Vector2 (ps.MoveSpeed * InputDirect, rb.velocity.y);
        }
        else if(!IsGrounded && !IsWallSlide && InputDirect != 0.0f)
        {
            Vector2 forceAdd_ = new Vector2(ps.AirForce * InputDirect, 0);

            rb.AddForce(forceAdd_);

            if(Mathf.Abs(rb.velocity.x) > ps.MoveSpeed)
            {
                rb.velocity = new Vector2(ps.MoveSpeed * InputDirect, rb.velocity.y);
            }
        }
        else if(!IsGrounded && !IsWallSlide && InputDirect == 0.0f)
        {
            rb.velocity = new Vector2(rb.velocity.x * ps.AirDrag, rb.velocity.y);
        }
        

        if(IsWallSlide)
        {
            if(rb.velocity.y < -WallSildeSpeed)
            {
                if (VerticalDirect < -0.01f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -WallSildeSpeed*5);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, -WallSildeSpeed);
                }
            }
        }
    }


    private void AnimationControl()
    {
        amt.SetBool("IsRun", IsRun);
        amt.SetBool("IsGrounded", IsGrounded);
        amt.SetFloat("yVelocity", rb.velocity.y);
        amt.SetBool("IsWallSlide", IsWallSlide);
        amt.SetFloat("yDirect", VerticalDirect);
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GroundCheck.position, GroundCheckRadius);
        Gizmos.DrawLine(WallCheck.position, new Vector3(WallCheck.position.x + WallCheckDistance, WallCheck.position.y, WallCheck.position.z));
    }


    public bool GetIsWallSlide()
    {
        return IsWallSlide;
    }

    public bool GetIsDash()
    {
        return IsDash;
    }

    public bool getIsGrounded()
    {
        return IsGrounded;
    }

    public Rigidbody2D GetRb()
    {
        return rb;
    }

    public void EnableInput()
    {
        CanInput = true;
    }

    public void DisableInput()
    {
        CanInput = false;
    }

    public float GetFlipDirect()
    {
        return FlipDirect;
    }

    public void GetFlipping()
    {
        Fliping();
    }

    public void EnableCanRun()
    {
        CanRun = true;
    }

    public void DisableCanRun()
    {
        CanRun = false;
    }
    
    public bool GetIsTouchWall()
    {
        return IsTouchWall;
    }

    public Animator GetAnimator()
    {
        return amt;
    }

    public CapsuleCollider2D GetCapsuleCollider()
    {
        return GetComponent<CapsuleCollider2D>();
    }
}
