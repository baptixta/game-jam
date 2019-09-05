using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recruit : MonoBehaviour
{
    public Transform recruiter;
    public float speed;
    public float accelerationSpeed;
    public bool inArea;
    Rigidbody2D rb;
    float velocityToApply;
    public Collider2D triggerToDeactivateWhenRecruited;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //Sprite flip
        if (rb.velocity.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        if (rb.velocity.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }

        if (recruiter == null) //Stay in place, no recruiter to follow
        {
            velocityToApply = Mathf.Lerp (velocityToApply, 0.0f, accelerationSpeed * Time.deltaTime);
            rb.velocity = new Vector2 (velocityToApply, rb.velocity.y);
            return;
        }

        //Needs to move towards recruiter
        if (!inArea)
        {
            //Recruiter is on the right
            if (transform.position.x < recruiter.position.x)
            {
                //Calculate velocityToApply
                velocityToApply = Mathf.Lerp (velocityToApply, speed + Vector3.Distance (transform.position, recruiter.position), accelerationSpeed * Time.deltaTime);
                //Move
                rb.velocity = new Vector2 (velocityToApply, rb.velocity.y);
            }
            //Recruiter is on the left
            if (transform.position.x > recruiter.position.x)
            {
                //Calculate velocityToApply
                velocityToApply = Mathf.Lerp (velocityToApply, -speed - Vector3.Distance (transform.position, recruiter.position), accelerationSpeed * Time.deltaTime);
                //Move
                rb.velocity = new Vector2 (velocityToApply, rb.velocity.y);
            }
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (recruiter != null)
        {
            if (other.gameObject.transform == recruiter)
            {
                inArea = true;
            }
        }
    }

    void OnTriggerExit2D (Collider2D other)
    {
        if (recruiter != null)
        {
            if (other.gameObject.transform == recruiter)
            {
                inArea = false;
            }
        }
    }
}
