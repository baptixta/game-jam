using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    //     EDITOR     //
    [Header ("Adjustments")]
    public float speed;
    public float jumpForce;
    public float respawnTime;
    public float horizontalForceDecreaseSpeed = 3.5f;
    [Header ("Ground Detection")]
    public LayerMask groundLayerMask;
    [Header ("Wall Jump")]
    public LayerMask wallLayerMask;
    public float wallJumpDrag;
    public float wallJumpForce;

    //     PRIVATE     //
    Rigidbody2D rb;
    SpriteRenderer[] renderersToFlip;
    Animator[] animators;
    bool jumpFlag;
    bool wallJumpFlag;
    float timeGrounded;
    Vector2 inputVector;
    Vector3 startPosition;
    bool wasGrounded = false;
    bool respawning = false;
    float customHorizontalForce;

    void Start()
    {
        //Getting references
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        renderersToFlip = GetComponentsInChildren<SpriteRenderer>();
        startPosition = transform.position;
    }

    void Update()
    {
        //Input vector
        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Walk
        if (!respawning)
        {
            rb.velocity = new Vector2(inputVector.x * speed, rb.velocity.y);
        }

        //Jump input flag
        if (Input.GetButton("Jump"))
        {
            jumpFlag = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            StopCoroutine(ResetJumpFlag());
            StartCoroutine(ResetJumpFlag());
        }

        //Wall jump flag
        if (HitLeftWall() || HitRightWall() && !IsGrounded())
        {
            if (Input.GetButtonDown("Jump"))
            {
                wallJumpFlag = true;
                StopCoroutine(ResetWallJumpFlag());
                StartCoroutine(ResetWallJumpFlag());
            }
        }

        //Sprite flip
        if (inputVector.x > 0)
        {
            foreach (SpriteRenderer sr in renderersToFlip)
            {
                sr.flipX = false;
            }
        }
        if (inputVector.x < 0)
        {
            foreach (SpriteRenderer sr in renderersToFlip)
            {
                sr.flipX = true;
            }
        }

        //Animations
        foreach (Animator a in animators)
        {
            a.SetBool("Walking", (inputVector.x != 0));
            a.SetBool("Grounded", IsGrounded());
        }
        
        //Ground collision sound
        if (!wasGrounded)
        {
            if (IsGrounded())
            {
                AudioManager.instance.PlaySound("Collision");
            }
        }
        wasGrounded = IsGrounded();

        //Decreasing customHorizontalForce
        customHorizontalForce = Mathf.Lerp (customHorizontalForce, 0, horizontalForceDecreaseSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        //Applying jump
        if (jumpFlag && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            rb.AddForce(Vector2.up * jumpForce);
            jumpFlag = false;
            AudioManager.instance.PlaySound ("Jump");
        }

        //Wall jump
        if ((HitLeftWall() && inputVector.x < 0) || (HitRightWall() && inputVector.x > 0))
        {
            rb.drag = 0.0f;
            if (!IsGrounded())
            {
                if (rb.velocity.y < 0)
                {
                    rb.drag = wallJumpDrag;
                }
                else
                {
                    rb.drag = 0.0f;
                }
                if (wallJumpFlag)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                    rb.AddForce(Vector2.up * jumpForce);
                    wallJumpFlag = false;
                    customHorizontalForce = inputVector.x * -wallJumpForce;
                }
            }
        }
        else
        {
            rb.drag = 0.0f;
        }

        //Applying customHorizontalForce if necessary
        if (customHorizontalForce != 0)
        {
            rb.velocity = new Vector2 (rb.velocity.x + customHorizontalForce, rb.velocity.y);
        }
    }

    #region GROUND DETECTION

    //This method returns true or false, depending on wether or not the player
    //is on top of any GameObject.
    bool IsGrounded()
    {
        return Physics2D.Raycast (transform.position, Vector2.down, (GetComponent<Collider2D>().bounds.size.y / 2) + 0.1f, groundLayerMask);
    }

    #endregion

    #region WALL DETECTION
    bool HitLeftWall ()
    {
        //Debug.DrawRay (transform.position, Vector2.left * ((GetComponent<Collider2D>().bounds.size.x / 2) + 0.1f), Color.blue, 0.1f);
        return Physics2D.Raycast (transform.position, Vector2.left, (GetComponent<Collider2D>().bounds.size.x / 2) + 0.1f, wallLayerMask);
    }

    bool HitRightWall ()
    {
        //Debug.DrawRay (transform.position, Vector2.right * ((GetComponent<Collider2D>().bounds.size.x / 2) + 0.1f), Color.blue, 0.1f);
        return Physics2D.Raycast (transform.position, Vector2.right, (GetComponent<Collider2D>().bounds.size.x / 2) + 0.1f, wallLayerMask);
    }
    #endregion

    #region COLLISIONS / TRIGGERS
    void OnCollisionEnter2D (Collision2D collision)
    {
        //Enemy
        if (collision.gameObject.tag == "Enemy")
        {
            StartCoroutine(Respawn());
        }
        //Death tag
        if (collision.gameObject.tag == "Death")
        {
            StartCoroutine(Respawn());
        }
    }

    #endregion

    #region COROUTINES
    IEnumerator ResetJumpFlag ()
    {
        yield return new WaitForSeconds(0.1f);
        jumpFlag = false;
    }

    IEnumerator ResetWallJumpFlag ()
    {
        yield return new WaitForSeconds(0.00f);
        wallJumpFlag = false;
    }

    IEnumerator Respawn ()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        transform.position = startPosition;
        respawning = true;
        yield return new WaitForSeconds(respawnTime);
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.position = startPosition;
        respawning = false;
    }

    #endregion

    #region ANIMATION EVENTS

    public void PlayStepSound ()
    {
        AudioManager.instance.SetPitch ("Step", Random.Range(0.9f, 1.1f));
        AudioManager.instance.PlaySound ("Step");
    }

    #endregion
}