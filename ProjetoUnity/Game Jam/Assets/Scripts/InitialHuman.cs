using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialHuman : MonoBehaviour
{
    public GameObject defaultVersion;
    public Transform defaultVersionSpawnpoint;

    public void AllowPlayerToWakeUp ()
    {
        PlayerMovement.instance.canWakeUp = true;
    }

    public void CallCameraShake ()
    {
        CameraBehaviour.instance.CallCameraAnimation ("Shake");
    }

    public void SetDefaultVersion ()
    {
        GameObject obj = Instantiate (defaultVersion, defaultVersionSpawnpoint.position, defaultVersionSpawnpoint.rotation);
        obj.GetComponentInChildren<SpriteRenderer>().flipX = true;
        obj.GetComponent<NPC>().autoFlip = false;
        Destroy (gameObject);
    }
}
