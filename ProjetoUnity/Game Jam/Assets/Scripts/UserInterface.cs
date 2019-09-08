using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{
    public Animator transitionAnimator;
    public static UserInterface instance;

    void Awake ()
    {
        instance = this;
    }

    public void CallTransition (string animationName)
    {
        transitionAnimator.SetTrigger (animationName);
    }
}
