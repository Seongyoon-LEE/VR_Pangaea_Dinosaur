using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TitanboaCtrl : MonoBehaviour
{
    private readonly int hashForward = Animator.StringToHash("Forward");
    private readonly int hashBack = Animator.StringToHash("Back");
    Animator animator;
    Rig rig;
    public bool isTirrger = false;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
    }

    public void OnBoaTigger(bool type)
    {
        if (isTirrger != type)
        {
            animator.SetTrigger(type ? hashForward : hashBack);
            rig.weight = type ? 1f : 0f;
            isTirrger = type;
        }
    }
}
