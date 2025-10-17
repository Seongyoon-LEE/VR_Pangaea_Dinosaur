using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class SpinosaurusCtrl : MonoBehaviour
{
    private readonly int hashTrace = Animator.StringToHash("isTrace");
    private readonly int hashAttack = Animator.StringToHash("isAttack");

    private enum Status
    {
        None, PATROL, TRACE, ATTACK
    }
    [SerializeField] private Status status = Status.None;

    Animator animator;
    NavMeshAgent agent;
    WaitForSeconds ws;
    private Transform playerTr;
    PatrolPoints path;
    int idx = 0;
    float moveSpeed = 1f;
    float rotSpeed = 10f;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ws = new WaitForSeconds(0.3f);
        path = GameObject.Find("Points").GetComponent<PatrolPoints>(); // 이름 변경
        StartCoroutine(DinoAppearedCoroutine()); // 호출 위치 나중에 변경
    }
   
    IEnumerator DinoAppearedCoroutine()
    {
        while(true)
        {
            yield return ws;

            switch (status)
            {
                case Status.PATROL:
                    OnPatrol();
                    break;
                case Status.TRACE:
                    OnTrace();
                    break;
                case Status.ATTACK:
                    OnAttack();
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
    public void FindOut(Transform tr)
    {
        status = Status.TRACE;
        playerTr = tr;
        agent.isStopped = false;
        agent.destination = playerTr.position;
    }

    void OnPatrol()
    {
        agent.isStopped = false;
        agent.destination = path.GetWayPoint(idx);

        if (Vector3.Distance(path.FlattenY(path.GetWayPoint(idx)), path.FlattenY(transform.position)) < 0.5f)
        {
            idx = path.CurrentWayPoint(idx);
        }
    }

    void OnTrace()
    {
        animator.SetBool(hashTrace, true);
        animator.SetBool(hashAttack, false);
        agent.isStopped = false;
        agent.destination = playerTr.position;
    }

    void OnAttack()
    {
        animator.SetBool(hashAttack, true);
        agent.isStopped = true;

        Vector3 taget = (playerTr.position - transform.position).normalized;

        Quaternion rot = Quaternion.LookRotation(taget);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotSpeed);
    }


}
