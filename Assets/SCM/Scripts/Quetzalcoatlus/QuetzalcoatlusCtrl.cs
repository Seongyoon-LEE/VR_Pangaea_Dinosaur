using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuetzalcoatlusCtrl : MonoBehaviour
{
    private readonly int hashLeftRight = Animator.StringToHash("LeftRight");
    private readonly int hashUpDown = Animator.StringToHash("UpDown");
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
    Animator animator;

    float dampTime = 3f;
    [SerializeField] float curTime = 0f;
    [SerializeField] float updownValue;
    [SerializeField] float leftrightValue;
    void Start()
    {
        path = GameObject.Find("Points").GetComponent<PatrolPoints>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        switch (status)
        {
            case Status.PATROL:
                OnPatrol();
                break;
            case Status.ATTACK:
                OnAttack();
                break;
        }
    }

    private void LateUpdate()
    {
        if (status == Status.ATTACK)
        {
            animator.SetFloat(hashLeftRight, 0.5f);
            animator.SetFloat(hashUpDown, 0.5f);
        }

        curTime += Time.deltaTime;

        if (curTime > dampTime)
        {
            updownValue = Random.value;
            leftrightValue = Random.value;
            curTime = 0f;
        }
        animator.SetFloat(hashLeftRight, leftrightValue, dampTime, Time.deltaTime);
        animator.SetFloat(hashUpDown, updownValue, dampTime, Time.deltaTime);
    }
    void OnPatrol()
    {
        Vector3 movePos = path.GetWayPoint(idx) - transform.position;
        movePos.y = 0f;

        if (movePos != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movePos), Time.fixedDeltaTime * rotSpeed);
        }

        transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
        if (Vector3.Distance(path.FlattenY(path.GetWayPoint(idx)), path.FlattenY(transform.position)) < 0.5f)
        {
            idx = path.CurrentWayPoint(idx);
        }
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
