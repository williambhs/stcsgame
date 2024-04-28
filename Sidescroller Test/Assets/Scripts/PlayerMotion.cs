using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float speed = 6;
    [SerializeField] private float jumpPower = 6;

    [Header("Debug Stuff")]
    [SerializeField] private float impulseX = 0.25f;
    [SerializeField] private float impulseY = 0.25f;
    [SerializeField] private float slideDamper = 1.0f;

    private Rigidbody2D body;
    private Animator animator;
    private GameObject bombInstance;
    private bool grounded;
    private bool isMovingDueToExplosion = false;
    private PlayerState playerState;
    private Countdown bombCooldown;

    // Start is called before the first frame update
    void Start()
    {
        //these grab references for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        playerState = PlayerState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        // First, update any timers, since this could affect the player state and
        // possible actions they can take.
        bombCooldown.Elapse(Time.deltaTime);

        UpdatePlayerState();
    }

    private void UpdatePlayerState()
    {
        // If we're no longer moving (for whatever reason), clear the isMovingDueToExplosion flag.
        if (body.velocity == Vector2.zero)
        {
            isMovingDueToExplosion = false;
        }

        float horizontalInput = Input.GetAxis("Horizontal");


        if (Input.GetKey(KeyCode.C))
        {
            Slide();
        }
        else
        {
            // The player must camp on the Slide key to stay in sliding state.
            // We do not move out of this state until the player releases that
            // key. Once they release that key, we clear the Sliding flag and
            // let the logic below determine the new state.
            ClearSlideState();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
        
        if (Input.GetKey(KeyCode.X))
        {     
            PlantBomb();
        }
        
        if (Input.GetKey(KeyCode.I))
        {
            Debug_Impulse();
        }

        if (!TryTransitionToRunning(horizontalInput))
        {
            TryTransitionToIdle();
        }

        //Set animator parameters
        animator.SetBool("grounded", grounded);

        //Debug.Log("Current State: " + playerState.ToString() + "  -  Grounded: " + grounded);
    }


    private bool TryTransitionToRunning(float horizontalInput)
    {
        if (horizontalInput != 0 &&
            isMovingDueToExplosion == false &&
            playerState != PlayerState.JumpingUp &&
            playerState != PlayerState.Sliding)
        {
            TryTransitionToState(PlayerState.Running);
            
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            // Flip player when moving in respective direction
            if (horizontalInput > 0.01f)
            {
                transform.localScale = Vector3.one;
            }
            else if (horizontalInput < -0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            animator.SetBool("run", true);

            return true;
        }

        return false;
    }

    private bool TryTransitionToIdle()
    {
        if (playerState != PlayerState.Sliding && 
            grounded)
        {
            TryTransitionToState(PlayerState.Idle);

            animator.SetBool("run", false);
            
            body.velocity = Vector2.zero;

            return true;
        }

        return false;
    }

    private void Debug_Impulse()
    {
        Vector2 force = new Vector2(impulseX * GetPlayerDirection(), impulseY);

        body.AddForce(force, ForceMode2D.Impulse);

        // Keep track of the fact that we're moving due to an explosion.
        isMovingDueToExplosion = true;
    }

    private void PlantBomb()
    {
        if (bombCooldown.HasExpired())
        {
            if (bombInstance != null)
            {
                Destroy(bombInstance);
            }

            bombInstance = Instantiate(bombPrefab);
            bombInstance.transform.position = new Vector3(transform.position.x + (0.5f * GetPlayerDirection()), transform.position.y, transform.position.z);
            
            var explosion = bombInstance.GetComponent<Explosion>();

            // here should be where bomb gets force applied to be thrown in direction player is looking
            var bombRigidBody = bombInstance.GetComponent<Rigidbody2D>();
            bombRigidBody.AddForce(new Vector2(5 * GetPlayerDirection(), 3), ForceMode2D.Impulse);

            explosion.Exploded += OnBombExploded;

            // We've planted a bomb, so reset to cooldown period.
            bombCooldown.Reset(5);
        }
    }


    private bool TryTransitionToState(PlayerState toState)
    {
        playerState = toState;
        return true;
    }

    private void Jump()
    {
        if (grounded && TryTransitionToState(PlayerState.JumpingUp))
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animator.SetTrigger("jump");
            grounded = false;
        }
    }

    private void Slide()
    {
        if (TryTransitionToState(PlayerState.Sliding))
        {
            animator.SetBool("sliding", true);

            // If we're sliding while on the ground, apply a little friction on the x axis.
            if (grounded)
            {
                body.velocity = new Vector2(body.velocity.x * slideDamper, body.velocity.y);

                //Debug.Log("Sliding Velocity: " + body.velocity.x);
            }
        }
    }

    private void ClearSlideState()
    {
        playerState = PlayerState.Unknown;

        animator.SetBool("sliding", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;

            UpdatePlayerState();
        }
    }

    private void OnBombExploded(Explosion explosion)
    {
        // Disconnect from the Exploded event. It only fires once. This prevents memory leaks.
        explosion.Exploded -= OnBombExploded;

        Vector2 force = explosion.GetAppliedForce(transform.position);

        body.AddForce(force, ForceMode2D.Impulse);

        // Keep track of the fact that we're moving due to an explosion.
        isMovingDueToExplosion = true;
    }

    private float GetPlayerDirection()
    {
        return transform.localScale.x;
    }

    private enum PlayerState
    {
        Unknown,

        Idle,
        Running,
        JumpingUp,
        Sliding,

        Count,
    }
}

struct Countdown
{
    public float RemainingTime
    {
        get
        {
            return remainingTime;
        }
    }

    public bool HasExpired()
    {
        return remainingTime <= 0;
    }

    public void Reset(float seconds)
    {
        remainingTime = seconds;
    }

    public void Elapse(float elapsedSeconds)
    {
        remainingTime -= elapsedSeconds;

        // Clamp to 0
        if (remainingTime < 0)
        {
            remainingTime = 0;
        }
    }

    private float remainingTime;
}
