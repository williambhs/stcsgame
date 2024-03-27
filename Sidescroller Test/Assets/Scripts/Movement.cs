using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private BoxCollider2D col;
    [SerializeField] private LayerMask jumpableGround;

    private float dirX;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 10f;

    // wallsliding shenanigans
    [SerializeField] private float wallSlidingSpeed = 0;
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector2 wallCheckSize;
    
    private bool isWalled;
    private bool isWallSliding;

    // dashing shenanigans
    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer tr;



    // called once
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
    }

    // called every frame
    private void Update()
    {
        if (isDashing)
        {
            return;
        }
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        UpdateAnimationUpdate();
        WallSlide();
        CheckWorld();
    }

    private void UpdateAnimationUpdate()
    {
        if (dirX > 0f)
        {
            anim.SetBool("running", true);
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            anim.SetBool("running", true);
            sprite.flipX = true;
        }
        else 
        {
            anim.SetBool("running", false);
        }
    }

    // to prevent infinite jumping
    private bool isGrounded()
    {
        return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, jumpableGround);

    }

    // wallsliding - https://www.youtube.com/watch?v=O6VX6Ro7EtA
    private void CheckWorld()
    {
        if (sprite.flipX)
        {
            isWalled = Physics2D.OverlapBox(wallCheckLeft.position, wallCheckSize, 0, wallLayer);
        }
        else
        {
            isWalled = Physics2D.OverlapBox(wallCheckRight.position, wallCheckSize, 0, wallLayer);
        }
    }

    private void WallSlide()
    {
        if (isWalled && !isGrounded() && dirX != 0f)
        {
            isWallSliding = true;
            
            //rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));        
        }    
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
        }
    }

    // dashing 
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        if (sprite.flipX)
        {
            rb.velocity = new Vector2(-1 * transform.localScale.x * dashingPower, 0f);
        }
        else
        {
            rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        }
        
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
