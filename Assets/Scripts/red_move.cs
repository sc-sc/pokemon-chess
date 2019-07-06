using UnityEngine;
using System.Collections;

public class red_move : MonoBehaviour
{

    public float speed;                //Floating point variable to store the player's movement speed.

    private Rigidbody2D rb2d;        //Store a reference to the Rigidbody2D component required to use 2D Physics.
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isSeeingUp = true;

    private float moveHorizontal = 0f;
    private float moveVertical = 0f;

    // Use this for initialization
    void Start()
    {
        //Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        //Store the current horizontal input in the float moveHorizontal.
        moveHorizontal = Input.GetAxis("Horizontal");
  
        //Store the current vertical input in the float moveVertical.
        moveVertical = Input.GetAxis("Vertical");

        if (moveVertical < 0)
        {
            isSeeingUp = false;
        } else if (moveVertical > 0)
        {
            isSeeingUp = true;
        }

        if (moveHorizontal > 0)
        {
            spriteRenderer.flipX = true;
        } else if (moveHorizontal < 0)
        {
            spriteRenderer.flipX = false;
        }

        //Use the two store floats to create a new Vector2 variable movement.
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        //Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
        rb2d.velocity = (movement * speed);

        animator.SetBool("isSeeingUp", isSeeingUp);
    }
}