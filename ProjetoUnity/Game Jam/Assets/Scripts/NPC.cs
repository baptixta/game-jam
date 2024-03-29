﻿    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    public bool autoFlip = true;
    public float minDelay;
    public float maxDelay;
    public GameObject DESTROCADOVersion;

    IEnumerator Start ()
    {
        yield return new WaitForSeconds (Random.Range(minDelay, maxDelay));
        if (autoFlip)
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            StartCoroutine (Start());
        }
    }

    public void DESTROCAR ()
    {
        GameObject obj = Instantiate (DESTROCADOVersion, transform.position, transform.rotation);
        foreach (Rigidbody2D rb in obj.GetComponentsInChildren<Rigidbody2D>())
        {
            rb.AddTorque (Random.Range(-50, 50));
        }
        if (SceneManager.GetActiveScene().name == "1")
        {
            if (Window.instance != null)
            Window.instance.canBeBroken = true;
        }
        Destroy (gameObject);
    }
}
