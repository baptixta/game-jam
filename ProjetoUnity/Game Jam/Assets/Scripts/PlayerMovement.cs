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
    public float killJumpForce;
    public float stompForce;
    public float respawnTime;
    public float flyForce;
    public float horizontalForceDecreaseSpeed = 3.5f;
    [Header ("Ground Detection")]
    public LayerMask groundLayerMask;
    [Header ("Wall Jump")]
    public LayerMask wallLayerMask;
    public float wallJumpDrag;
    public float wallJumpForce;
    public bool canWakeUp = false;
    public GameObject cabeca;
    public static PlayerMovement instance;

    //     PRIVATE     //
    Rigidbody2D rb;
    SpriteRenderer[] renderersToFlip;
    Animator animator;
    bool jumpFlag;
    bool wallJumpFlag;
    float timeGrounded;
    Vector2 inputVector;
    Vector3 startPosition;
    bool wasGrounded = false;
    bool respawning = false;
    bool stomping = false;
    bool stunned = false;
    float customHorizontalForce;
    public bool fly = false;

    public GameObject equippedHat;

    void Awake ()
    {
        instance = this;
    }

    void Start()
    {
        //Getting references
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        renderersToFlip = GetComponentsInChildren<SpriteRenderer>();
        startPosition = transform.position;
    }

    void Update()
    {
        if (!canWakeUp)
        {
            return;
        }

        //Stunned
        if (stunned)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool ("Stomping", true);
            return;
        }

        //Wake up
        if (Input.anyKeyDown && canWakeUp)
        {
            animator.SetTrigger ("WakeUp");
            CameraBehaviour.instance.desiredSize = 5.0f;
            CameraBehaviour.instance.offset = new Vector3 (0, 1, -10);
        }

        //Input vector
        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Walk
        if (!respawning)
        {
            rb.velocity = new Vector2(inputVector.x * speed, rb.velocity.y);
        }

        //Jump input flag
        if (!fly)
        {
            if (!stomping)
            {
                if (Input.GetButton("Jump"))
                {
                    jumpFlag = true;
                }
                if (Input.GetButtonUp("Jump"))
                {
                    StopCoroutine(ResetJumpFlag());
                    StartCoroutine(ResetJumpFlag());
                }
            }
        }
        else //Fly
        {
            if (Physics2D.Raycast(transform.position, Vector2.down, 5.0f, groundLayerMask))
            {
                if (Input.GetButton ("Jump"))
                {
                    rb.AddForce (Vector2.up * flyForce);
                }
            }
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

        //Stomp flag
        if (!IsGrounded() && Input.GetButtonDown ("Stomp") && !stomping)
        {
            StartCoroutine (Stomp(0.3f));
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
        animator.SetBool("Stomping", stomping);
        animator.SetBool("Walking", (inputVector.x != 0));
        animator.SetBool("Grounded", IsGrounded());
        
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

        //Stomping
        if (stomping)
        {
            rb.velocity = new Vector2 (0.0f, rb.velocity.y);
        }
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

        if (GetGroundObject() != null)
        {
            //DESTROÇANDO Humano
            if (GetGroundObject().GetComponent<NPC>())
            {
                GetGroundObject().GetComponent<NPC>().DESTROCAR();
                rb.velocity = new Vector2 (rb.velocity.x, -rb.velocity.y);
                //rb.AddForce (Vector2.up * killJumpForce);
            }
        }

        //Applying customHorizontalForce if necessary
        if (customHorizontalForce != 0)
        {
            rb.velocity = new Vector2 (rb.velocity.x + customHorizontalForce, rb.velocity.y);
        }

        //Stopping stomp
        if (stomping)
        {
            if (IsGrounded())
            {
                stomping = false;
                StartCoroutine(Stun(0.5f));
                CameraBehaviour.instance.CallCameraAnimation("Shake");
            }
        }
    }

    #region GROUND DETECTION

    //This method returns true or false, depending on wether or not the player
    //is on top of any GameObject.
    bool IsGrounded()
    {
        //return Physics2D.Raycast (transform.position, Vector2.down, (GetComponent<Collider2D>().bounds.size.y / 2) + 0.1f, groundLayerMask);
        return (GetGroundObject() != null);
    }

    GameObject GetGroundObject ()
    {
        RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.down, (GetComponent<Collider2D>().bounds.size.y / 2) + 0.1f, groundLayerMask);
        if (hit.collider != null)
            return hit.collider.gameObject;
        else
            return null;
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
        if (collision.gameObject.name == "megazord")
        {
            collision.gameObject.GetComponent<PlayerMovement>().cabeca.SetActive (true);
            CameraBehaviour.instance.player = collision.gameObject.transform;
            instance = collision.gameObject.GetComponent<PlayerMovement>();
            collision.gameObject.GetComponent<PlayerMovement>().enabled = true;
            Destroy (gameObject);
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.name == "Hat")
        {
            Destroy (other.gameObject);
            equippedHat.SetActive (true);
            fly = true;
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

    IEnumerator Stomp (float delay)
    {
        stomping = true;
        rb.velocity = new Vector2 (rb.velocity.x, 5);
        rb.gravityScale = 0.5f;
        yield return new WaitForSeconds (delay);
        rb.gravityScale = 1.0f;
        rb.velocity = new Vector2 (rb.velocity.x, 0.0f);
        rb.AddForce (Vector2.down * stompForce);
    }

    IEnumerator Stun (float seconds)
    {
        stunned = true;
        yield return new WaitForSeconds (seconds);
        stunned = false;
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