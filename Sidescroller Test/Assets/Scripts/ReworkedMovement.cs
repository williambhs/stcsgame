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

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //these grab references for rigidbody and animator from object
       
    }

    // Update is called once per frame
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
         body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        
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
            Jump();
        }

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", grounded);
    }

   private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpPower);
        anim.SetTrigger("jump");
        grounded = false;

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

            explosion.Exploded += OnBombExploded;

            bombIsActive = true;
        }
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

        float relativePower = explosion.GetRelativePower(transform.position);

        // There's extra math needed here for the x and y axes.
        body.velocity += new Vector2(relativePower, relativePower);

        bombIsActive = false;
    }


    //private void getDistance()
    //{
    //    distanceX = player.transform.position.x - transform.position.x;
    //    distanceY = player.transform.position.y - transform.position.y;

    //}
    
}
