using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitanboaCtrl : MonoBehaviour
{
    Animator animator;
    public bool isTirrger = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (isTirrger)
        {

        }
    }

    public void OnBoaTigger()
    {
        isTirrger = true;
        // 애니메이션으로 등장
    }


}
