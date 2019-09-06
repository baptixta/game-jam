using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public bool canBeBroken = false;
    public GameObject brokenPrefab;

    public void Break ()
    {
        if (canBeBroken)
        {
            Instantiate (brokenPrefab, transform.position, transform.rotation);
            Destroy (gameObject);
        }
    }

    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Break();
        }
    }
}
