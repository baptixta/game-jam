using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public float minDelay;
    public float maxDelay;
    public GameObject DESTROCADOVersion;

    IEnumerator Start ()
    {
        yield return new WaitForSeconds (Random.Range(minDelay, maxDelay));
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        StartCoroutine (Start());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Instantiate (DESTROCADOVersion, transform.position, transform.rotation);
            Destroy (gameObject);
        }
    }
}
