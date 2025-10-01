using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuetzalcoatlusCtrl : MonoBehaviour
{
    private enum Status
    {
        PATROL, ATTACK
    }

    [SerializeField] private Status status = Status.PATROL;

    PatrolPoints path;
    int idx = 0;
    float moveSpeed = 1f;
    float rotSpeed = 10f;
    private Transform playerTr;
    void Start()
    {
        path = GameObject.Find("Points").GetComponent<PatrolPoints>();
    }

    void FixedUpdate()
    {
        switch (status)
        {
            case Status.PATROL:
                //OnPatrol();
                break;
            case Status.ATTACK:
                OnAttack();
                break;
        }
    }

    void OnPatrol()
    {
        Vector3 movePos = path.GetWayPoint(idx) - transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movePos), Time.fixedDeltaTime * rotSpeed);
        transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(path.GetWayPoint(idx), transform.position) < 0.5f)
        {
            idx = path.CurrentWayPoint(idx);
        }
    }

    IEnumerator HeadLookCoroutine()
    {


        yield return new WaitForSeconds(3f);

        HeadLook();
    }

    void HeadLook()
    {
        StartCoroutine(HeadLookCoroutine());
    }

    void OnAttack()
    {
        if (playerTr == null) return;
        Vector3 movePos = playerTr.position - transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movePos), Time.fixedDeltaTime * rotSpeed);
        transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
    }

    public void FindOut(Transform tr)
    {
        status = Status.ATTACK;
        playerTr = tr;
        moveSpeed = 5f;
    }
}
