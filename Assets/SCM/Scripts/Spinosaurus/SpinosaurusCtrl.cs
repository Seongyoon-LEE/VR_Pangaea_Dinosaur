using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class SpinosaurusCtrl : MonoBehaviour, IDinoCtrl
{
    private readonly int hashWalk = Animator.StringToHash("Walk");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");

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
    public float runSpeed = 5f;
    float rotSpeed = 10f;


    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ws = new WaitForSeconds(0.3f);
        path = GameObject.Find("Points").GetComponent<PatrolPoints>(); // 이름 변경
        //StartCoroutine(UpdateCurrentStatus()); // 호출 위치 나중에 변경
    }

    public IEnumerator UpdateCurrentStatus()
    {
        while (true)
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
                    OnIdle();
                    break;
            }
        }
    }

    public void DinoAppeared()
    {
        StartCoroutine(UpdateCurrentStatus());
        status = Status.PATROL;
    }
    public void FindOut(Transform tr)
    {
        status = Status.TRACE;
        playerTr = tr;
        agent.isStopped = false;
        agent.destination = playerTr.position;
    }

    public void OnPatrol()
    {
        agent.isStopped = false;
        agent.destination = path.GetWayPoint(idx);
        animator.SetFloat(hashWalk, 0.5f);
        if (Vector3.Distance(path.FlattenY(path.GetWayPoint(idx)), path.FlattenY(transform.position)) < 5f)
        {
            idx = path.CurrentWayPoint(idx);
        }
    }

    public void OnTrace()
    {
        if (Vector3.Distance(path.FlattenY(playerTr.position), path.FlattenY(transform.position)) < 5f)
        {
            status = Status.ATTACK;
            return;
        }

        animator.SetFloat(hashWalk, 1f);
        animator.SetBool(hashAttack, false);
        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.destination = playerTr.position;
    }

    public void OnAttack()
    {
        if (playerTr == null) return;

        animator.SetBool(hashAttack, true);
        agent.isStopped = true;

        Vector3 taget = (playerTr.position - transform.position).normalized;

        Quaternion rot = Quaternion.LookRotation(taget);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotSpeed);

        // 공격시 즉사
        // 사망 처리
        // 범위에서 벗어났을 때 추격 구현 필요X
    }

    public void OnIdle()
    {
        
    }
}
