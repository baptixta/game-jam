using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothTime;
    public bool active = false;
    public static CameraBehaviour instance;
    Vector3 currentVelocity;

    void Awake()
    {
        //Static instance reference
        instance = this;
    }

    void FixedUpdate()
    {
        //Interpolating position
        transform.position = Vector3.SmoothDamp (transform.position, player.position + offset, ref currentVelocity, smoothTime);
    }
}