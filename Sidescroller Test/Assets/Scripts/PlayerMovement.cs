using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject sideSensor;
    [SerializeField] private GameObject bottomSensor;

    [SerializeField] private float speed = 6;
    [SerializeField] private float jumpPower = 6;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Debug Stuff")]
    [SerializeField] private float impulseX = 0.25f;
    [SerializeField] private float impulseY = 0.25f;
    [SerializeField] private float slideDamper = 1.0f;
    [SerializeField] private float wallJumpXVelocity = 10.0f;

    private Rigidbody2D body;
    private Animator animator;
    private GameObject bombInstance;
    private bool isTouchingGround;
    private bool isTouchingWall;
    private bool isWallSliding;
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

        UpdateIsTouchingGround();
        UpdateIsTouchingWall();
        UpdateIsWallSliding();

        CommonScene.PrintDebugText($"Ground: {isTouchingGround} - Wall: {isTouchingWall} - Wall S: {isWallSliding} - Jumping Up: {(playerState == PlayerState.JumpingUp)}");
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
        animator.SetBool("grounded", isTouchingGround);

        //CommonScene.PrintDebugText("Current State: " + playerState.ToString() + "  -  Grounded: " + isTouchingGround);
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
                SetPlayerDirection(1);
            }
            else if (horizontalInput < -0.01f)
            {
                SetPlayerDirection(-1);
            }

            animator.SetBool("run", true);

            return true;
        }

        return false;
    }

    private bool TryTransitionToIdle()
    {
        if (playerState != PlayerState.Sliding &&
            isTouchingGround)
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
        if ((isTouchingGround || isTouchingWall) && 
            TryTransitionToState(PlayerState.JumpingUp))
        {
            isTouchingGround = false;

            float jumpXVelocity;

            if (isTouchingWall)
            {
                // -1 will make us jump away from the wall.
                jumpXVelocity = -1 * 5 * GetPlayerDirection();

                FlipPlayerDirection();
            }
            else
            {
                jumpXVelocity = body.velocity.x;
            }

            body.velocity = new Vector2(jumpXVelocity, jumpPower);
            animator.SetTrigger("jump");
        }
    }

    private void Slide()
    {
        if (TryTransitionToState(PlayerState.Sliding))
        {
            animator.SetBool("sliding", true);

            // If we're sliding while on the ground, apply a little friction on the x axis.
            if (isTouchingGround)
            {
                body.velocity = new Vector2(body.velocity.x * slideDamper, body.velocity.y);

                //Debug.Log("Sliding Velocity: " + body.velocity.x);
            }
        }
    }

    private void ClearSlideState()
    {
        if (playerState == PlayerState.Sliding)
        {
            TryTransitionToState(PlayerState.Unknown);

            animator.SetBool("sliding", false);
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

    private int GetPlayerDirection()
    {
        return (int)transform.localScale.x;
    }

    private void SetPlayerDirection(int direction)
    {
        var scale = transform.localScale;
        scale.x = direction;

        transform.localScale = scale;
    }

    private void FlipPlayerDirection()
    {
        SetPlayerDirection(GetPlayerDirection() * -1);
    }

    private void UpdateIsTouchingGround()
    {
        isTouchingGround = Physics2D.OverlapCircle(bottomSensor.transform.position, 0.1f, obstacleLayer);
    }

    private void UpdateIsTouchingWall()
    {
        isTouchingWall = Physics2D.OverlapCircle(sideSensor.transform.position, 0.1f, obstacleLayer);
    }

    private void UpdateIsWallSliding()
    {
        if (isTouchingWall && body.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
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
