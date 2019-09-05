using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recruiter : MonoBehaviour
{
    public Recruit nearbyRecruit;

    void Update ()
    {
        //Recruiting
        if (nearbyRecruit != null)
        {
            if (nearbyRecruit.recruiter == null)
            {
                if (Input.GetKeyDown (KeyCode.E))
                {
                    nearbyRecruit.recruiter = transform;
                    nearbyRecruit.triggerToDeactivateWhenRecruited.enabled = false;
                    nearbyRecruit = null;
                }
            }
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.GetComponent<Recruit>())
        {
            nearbyRecruit = other.gameObject.GetComponent<Recruit>();
        }
    }

    void OnTriggerExit2D (Collider2D other)
    {
        if (other.gameObject.GetComponent<Recruit>())
        {
            nearbyRecruit = null;
        }
    }
}