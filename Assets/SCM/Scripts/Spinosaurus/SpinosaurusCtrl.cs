using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinosaurusCtrl : MonoBehaviour
{
    private enum Status
    {
        None, PATROL, TRACE, ATTACK
    }
    private Status status = Status.None;

    Animator animator;
    WaitForSeconds ws;

    void Start()
    {
        animator = GetComponent<Animator>();
        ws = new WaitForSeconds(0.3f);
    }
   
    IEnumerator DinoAppearedCoroutine()
    {
        while(true)
        {
            yield return ws;

            switch (status)
            {
                case Status.PATROL:
                    break;
                case Status.TRACE:
                    break;
                case Status.ATTACK:
                    break;
                default:
                    break;
            }
        }
    }

    public void DinoAppeared()
    {
        StartCoroutine(DinoAppearedCoroutine());
        status = Status.PATROL;
    }
    public void FindOut()
    {
        status = Status.TRACE;
    }


}
