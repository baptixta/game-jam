using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public float minDelay;
    public float maxDelay;

    IEnumerator Start ()
    {
        yield return new WaitForSeconds (Random.Range(minDelay, maxDelay));
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        StartCoroutine (Start());
    }
}
