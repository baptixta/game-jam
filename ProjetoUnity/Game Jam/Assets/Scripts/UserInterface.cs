using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    public Animator transitionAnimator;

    public void CallTransition (string animationName)
    {
        transitionAnimator.SetTrigger (animationName);
    }
}
