using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recruit : MonoBehaviour
{
    public Transform recruiter;
    public Vector3 offset;
    public float speed;
    public float accelerationSpeed;
    public bool inArea;
    Rigidbody2D rb;
    float velocityToApply;
    public Collider2D triggerToDeactivateWhenRecruited;
    Transform pointer;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        //Instantiating pointer
        pointer = new GameObject ("Pointer").transform;
        pointer.SetParent (transform);
        pointer.localPosition = Vector3.zero;
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
            //Calculate velocityToApply;
            velocityToApply = Mathf.Lerp (velocityToApply, 0.0f, accelerationSpeed * Time.deltaTime);
            //Apply velocity
            rb.velocity = pointer.forward * velocityToApply;
            return;
        }

        //Pointer
        pointer.LookAt (recruiter.position + offset);

        //Needs to move towards recruiter
        if (!inArea)
        {
            //Calculate velocityToApply
            velocityToApply = Mathf.Lerp (velocityToApply, speed + Vector3.Distance (transform.position, recruiter.position), accelerationSpeed * Time.deltaTime);
        }

        //Apply velocity
        if (Vector3.Distance(transform.position, recruiter.position + offset) > 3)
        {
            rb.velocity = pointer.forward * velocityToApply;
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
