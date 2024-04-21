using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class ReworkedMovement : MonoBehaviour
{

    [SerializeField] private GameObject bombPrefab;

    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    public Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    private bool bombIsActive = false;
    private GameObject bombInstance;
    private bool slideReady;
    private float currentSpeed;
    private float hinderanceTimeRemaining = 0;
    private bool hinderance;
    //checks how much longer the player should be sliding for. This is for animation
    private float slideTimeRemaining = 0;
    //checks how much longer until player can slide again
    private float slideReadyTimeRemaining = 0;
    private bool isSliding;
    [SerializeField] private float slidePower;

    private void Awake()
    {
        //these grab references for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        isSliding = false;
        slideReady = true;
        isSliding = false;
        hinderance = false;

    }
    // Start is called before the first frame update
    void Start()
    {
       
       
    }

    // Update is called once per frame
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        /*if (horizontalInput != 0)
        {
            if (hinderance == true)
            {
                body.velocity = new Vector2(horizontalInput * speed - 4, body.velocity.y);
            }
            else
            {
                body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            } 
        }*/
        
        //flip player when moving in respective direction
        if (horizontalInput > 0.01f)
        {
           transform.localScale = Vector3.one;
        }
        else if(horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }


        //checking to see if space is pressed so player can jump
       if (Input.GetKey(KeyCode.Space) && grounded)
        {
            //jumping will reset slide cooldown instantly
            isSliding = false;
            slideReady = true;
            Jump();
        }

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", grounded);

        //checking to see if c is pressed so player can slide;
        if (Input.GetKey(KeyCode.C) && slideReady == true)
        {
            slideReady = false;
            isSliding = true;
            Slide();
        }
        
        //checking if the player is sliding for animation 
        if (isSliding == true)
        {
            if (slideTimeRemaining > 0)
            {
                slideTimeRemaining -= Time.deltaTime;
            }

            else
            {
                slideTimeRemaining = 0;
                isSliding = false;
                slideReady = false;
                slideReadyTimeRemaining = 2;
                anim.SetBool("sliding", false);
            }
        }
        //checking if player can slide again
        if (slideReady == false)
        {
            if (slideReadyTimeRemaining > 0)
            {
                slideReadyTimeRemaining -= Time.deltaTime;
            }
            else
            {
                slideReadyTimeRemaining = 0;
                slideReady = true;
      
            }
        }
        //checking if hinderance from jumping should still be applied to speed
        /*if (hinderance == true)
        {
            if (hinderanceTimeRemaining > 0)
            {
                hinderanceTimeRemaining -= Time.deltaTime;
            }
            else
            {
                hinderanceTimeRemaining = 0;
                hinderance = false;

            }
        }*/
    }

   private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpPower);
        anim.SetTrigger("jump");
        grounded = false;
        /*hinderance = true;
        hinderanceTimeRemaining = 1.25f;*/

        if (!bombIsActive)
        {
            // If there was a bomb in the scene, it is no longer active. So remove it before
            // creating a new instance.

            if (bombInstance != null)
            {
                Destroy(bombInstance);
            }

            bombInstance = Instantiate(bombPrefab);

            var explosion = bombInstance.GetComponent<Explosion>();

            bombInstance.transform.position = transform.position;

            explosion.Exploded += OnBombExploded;

            bombIsActive = true;
        }
    }

    private void Slide()
    {
        /*hinderance = false;
        hinderanceTimeRemaining = 0;*/
        anim.SetBool("sliding", true);
        if (slidePower == 1)
        {
            Vector2 slideForce = new Vector2(10, 0);
            body.AddForce(slideForce);
                
        }

        else
        {
            body.velocity += new Vector2(body.velocity.x * 8, 0);
        }

        isSliding = true;
        slideTimeRemaining = 0.5f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }

    }

    private void OnBombExploded(Explosion explosion)
    {
        // Disconnect from the Exploded event. It only fires once. This prevents memory leaks.
        explosion.Exploded -= OnBombExploded;

        // Do something with this event.

        //Vector2 powers = explosion.GetRelativePower(transform.position);

        //// There's extra math needed here for the x and y axes.
        ////body.velocity += new Vector2(relativePower, relativePower);
        //body.velocity += powers;

        Vector2 force = explosion.GetAppliedForce(transform.position);

        body.AddForce(force, ForceMode2D.Impulse);
        
        bombIsActive = false;
    }

    
    //private void getDistance()
    //{
    //    distanceX = player.transform.position.x - transform.position.x;
    //    distanceY = player.transform.position.y - transform.position.y;

    //}
    
}
