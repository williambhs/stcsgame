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
    [SerializeField] private LayerMask obstacleLayer;

    [SerializeField] private Vector2 sideSensorSize = new Vector2(0.2f, 0.4f);
    [SerializeField] private Vector2 bottomSensorSize = new Vector2(0.3f, 0.3f);

    [Header("Motion Stuff")]
    [SerializeField] private float speed = 6;
    [SerializeField] private float jumpPower = 6; 
    [SerializeField] private float groundSlideDamper = 1.0f;
    [SerializeField] private float wallSlideDamper = 1.0f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(5f, 5f);
    // User is frozen from influencing x movement during this time.
    [SerializeField] private float wallJumpingDuration = 0.5f;


    [Header("Debug Stuff")]
    [SerializeField] private float impulseX = 0.25f;
    [SerializeField] private float impulseY = 0.25f;

    private Rigidbody2D body;
    private Animator animator;
    private GameObject bombInstance;
    private bool isTouchingGround;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isMovingDueToExplosion = false;
    private PlayerState playerState;
    private Countdown bombCooldown;
    private Countdown canWallJumpCountdown;
    private Countdown playerLockedDueToWallJumpCountdown;

    private float wallJumpingTime = 0.2f;
    private float horizontalInput;

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
        playerLockedDueToWallJumpCountdown.Elapse(Time.deltaTime);

        // Only use the horzontal input if the player isn't locked due to
        // recently starting a wall jump.
        if (playerLockedDueToWallJumpCountdown.HasExpired())
        {
            horizontalInput = Input.GetAxis("Horizontal");
        }
        else
        {
            horizontalInput = 0;
        }

        UpdateIsTouchingGround();
        UpdateIsTouchingWall();
        UpdateIsWallSliding();

        if (CheckSliding())
        {
        }
        else if (CheckWallJumping())
        {
        }
        else if (CheckJumping())
        {
        }
        else if (CheckRunning())
        {
        }
        else
        {
            CheckIdle();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            PlantBomb();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            Debug_Impulse();
        }

        // We need to check both isTouchingGround AND y velocity is not going upwards. 
        animator.SetBool("grounded", isTouchingGround && body.velocity.y <= 0);

        CommonScene.PrintDebugText($"Player State: {playerState}\t - Grounded: {isTouchingGround}");
    }

    private bool CheckSliding()
    {
        if (Input.GetKey(KeyCode.C))
        {
            SetPlayerState(PlayerState.Sliding);
            
            animator.SetBool("sliding", true);

            // If we're sliding while on the ground, apply a little friction on the x axis.
            if (isTouchingGround)
            {
                SetPlayerVelocity(new Vector2(body.velocity.x * groundSlideDamper, body.velocity.y));

                //Debug.Log("Sliding Velocity: " + body.velocity.x);
            }

            return true;
        }
        else
        {
            // The player must camp on the Slide key to stay in sliding state.
            // We do not move out of this state until the player releases that
            // key. Once they release that key, we clear the Sliding flag and
            // let the logic below determine the new state.
            ClearSlideState();
        }

        return false;
    }

    private bool CheckWallJumping()
    {
        if (isWallSliding)
        {
            playerLockedDueToWallJumpCountdown.Stop();

            canWallJumpCountdown.Reset(wallJumpingTime);
        }
        else
        {
            canWallJumpCountdown.Elapse(Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && 
            !canWallJumpCountdown.HasExpired())
        {
            SetPlayerState(PlayerState.JumpingUp);

            playerLockedDueToWallJumpCountdown.Reset(wallJumpingDuration);

            canWallJumpCountdown.Stop();

            // -1 will make us jump away from the wall.
            float jumpXVelocity = -1 * wallJumpingPower.x * GetPlayerDirection();

            SetPlayerVelocity(new Vector2(jumpXVelocity, wallJumpingPower.y));

            FlipPlayerDirection();

            // Clear the horizontal input so that it doesn't interrupt the initial wall jumping.
            horizontalInput = 0;

            animator.SetTrigger("jump");

            return true;
        }

        return false;
    }

    private bool CheckJumping()
    {
        bool jumping = false;

        if (Input.GetKeyDown(KeyCode.Space) &&
            isTouchingGround)
        {
            SetPlayerState(PlayerState.JumpingUp);
        
            isTouchingGround = false;

            SetPlayerVelocity(new Vector2(body.velocity.x, jumpPower));

            animator.SetTrigger("jump");

            // If the jump key was also lifted this frame, then this was a light tap,
            // so dampen the jump power.
            if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0f)
            {
                SetPlayerVelocity(new Vector2(body.velocity.x, body.velocity.y * 0.5f));
            }

            jumping = true;
        }

        return jumping;
    }

    private bool CheckRunning()
    {
        if (isTouchingGround && body.velocity.x != 0 && horizontalInput != 0)
        {
            // We only update the player state and animation here. The position is
            // updated during FixedUpdate.
            SetPlayerState(PlayerState.Running);

            animator.SetBool("run", true);

            return true;
        }

        return false;
    }

    private bool CheckIdle()
    {
        if (playerState != PlayerState.Sliding &&
            isTouchingGround &&
            ((horizontalInput == 0) ||
             (horizontalInput != 0 && body.velocity.x == 0)))
        {
            SetPlayerState(PlayerState.Idle);

            animator.SetBool("run", false);

            return true;
        }

        return false;
    }


    private void FixedUpdate()
    {
        var expired = playerLockedDueToWallJumpCountdown.HasExpired();

        if (expired)
        {
            float velocityX;

            if (isTouchingGround || horizontalInput != 0)
            {
                velocityX = horizontalInput * speed;
            }
            else
            {
                velocityX = body.velocity.x;
            }

            SetPlayerVelocity(new Vector2(velocityX, body.velocity.y));

            // Flip player when moving in respective direction
            if (horizontalInput > 0.01f)
            {
                SetPlayerDirection(1);
            }
            else if (horizontalInput < -0.01f)
            {
                SetPlayerDirection(-1);
            }
        }
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

    private void SetPlayerState(PlayerState toState)
    {
        playerState = toState;
    }

    private void ClearSlideState()
    {
        if (playerState == PlayerState.Sliding)
        {
            SetPlayerState(PlayerState.Unknown);

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
        isTouchingGround = Physics2D.OverlapBox(bottomSensor.transform.position, bottomSensorSize, 0, obstacleLayer);
    }

    private void UpdateIsTouchingWall()
    {
        isTouchingWall = Physics2D.OverlapBox(sideSensor.transform.position, sideSensorSize, 0, obstacleLayer);
    }

    private void UpdateIsWallSliding()
    {
        if (isTouchingWall && 
            !isTouchingGround && 
            horizontalInput != 0)
        {
             isWallSliding = true;

            // Maybe slow down y velocity here.
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void SetPlayerVelocity(Vector2 velocity)
    {
        body.velocity = velocity;
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
