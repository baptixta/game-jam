using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Song : MonoBehaviour
{
    public AudioSource intro;
    public AudioSource loop;

    public void Start ()
    {
        intro.Play();
        StartCoroutine (PlayLoop());
    }

    IEnumerator PlayLoop ()
    {
        yield return new WaitForSeconds (intro.clip.length);
        loop.Play();
    }
}
