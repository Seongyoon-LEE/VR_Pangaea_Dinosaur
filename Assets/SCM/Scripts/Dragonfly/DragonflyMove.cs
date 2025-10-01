using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class DragonflyMove : MonoBehaviour
{
    private readonly int hashTrace = Animator.StringToHash("isTrace");
    private readonly int hashAttack = Animator.StringToHash("isAttack");
    private readonly string torchTag = "TORCH";
    private enum Status
    {
        None, TRACE, ATTACK, ESCAPE, RETURN
    }

    [SerializeField] private Status status = Status.None;
    private Transform playerTr;
    Animator animator;
    NavMeshAgent agent;
    WaitForSeconds ws;
    WaitForSeconds escapeSeconds;

    float traceRange = 100f;
    [SerializeField] float attackRange = 10f;
    float escapeRange = 10f;
    float rotSpeed = 10f;
    bool isTorch = false;
    int idx;
    float stoppingDistance = 3f;
    PatrolPoints path;
    void Start()
    {
        playerTr = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ws = new WaitForSeconds(0.5f);
        escapeSeconds = new WaitForSeconds(3f);

        StartCoroutine(StatusCheck());
        StartCoroutine(FindPlayer());
        idx = Random.Range(0, 3);
        path = GameObject.Find("DragonflyPoints").GetComponent<PatrolPoints>();
        transform.position = path.GetWayPoint(idx);
    }

    IEnumerator StatusCheck()
    {
        while (true)
        {
            yield return ws;

            switch (status)
            {
                case Status.TRACE:
                    OnTrace();
                    break;
                case Status.ATTACK:
                    OnAttack();
                    break;
                case Status.ESCAPE:
                    StartCoroutine(OnEscape());
                    break;
                case Status.RETURN:
                    if (agent.remainingDistance < 0.5f)
                    {
                        agent.stoppingDistance = stoppingDistance;
                        status = Status.None;
                        isTorch = false;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(torchTag))
        {
            isTorch = true;
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

    // 플레이어 반대 방향으로 도망가는 로직
    IEnumerator OnEscape()
    {
        animator.SetBool(hashAttack, false);
        animator.SetBool(hashTrace, true);
        agent.isStopped = false;

        
        Vector3 dist = (transform.position - playerTr.position).normalized;
        Vector3 targetDestination = transform.position + dist * escapeRange;

        if (NavMesh.SamplePosition(targetDestination, out NavMeshHit hit, escapeRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        yield return escapeSeconds;

        gameObject.SetActive(false);
    }

    // 포인트로 복귀하는 로직
    void OnReturn()
    {
        animator.SetBool(hashAttack, false);
        animator.SetBool(hashTrace, true);
        agent.isStopped = false;
        float dist = 0;

        for (int i = 0; i < path.GetWayCount(); i++)
        {
            float temp = (path.GetWayPoint(i) - transform.position).sqrMagnitude;
            if (temp > dist)
            {
                dist = temp;
                idx = i;
            }
        }

        agent.stoppingDistance = 0f;
        agent.destination = path.FlattenY(path.GetWayPoint(idx));
    }
    IEnumerator FindPlayer()
    {
        while(true)
        {
            yield return ws;

            if (status == Status.RETURN) continue;

            float dist = (playerTr.position - transform.position).sqrMagnitude;

            if (dist < attackRange)
            {
                status = Status.ATTACK;
            }
            else if (dist < traceRange)
            {
                status = Status.TRACE;
            }
            else
            {
                status = Status.None;
            }

            if (isTorch)
            {
                // 회피
                //status = Status.ESCAPE;
                //break;

                // 복귀
                status = Status.RETURN;
                OnReturn();
            }
        }
    }
}
