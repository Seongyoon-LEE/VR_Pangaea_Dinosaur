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
    public enum Status
    {
        None, TRACE, ATTACK, ESCAPE, RETURN
    }

    public Status status = Status.None;
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
    public int idx;
    float stoppingDistance = 3f;
    public bool isPosition = false;
    PatrolPoints path;
    IEnumerator Start()
    {
        playerTr = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ws = new WaitForSeconds(0.5f);
        escapeSeconds = new WaitForSeconds(3f);

        StartCoroutine(StatusCheck());
        StartCoroutine(FindPlayer());
        path = GameObject.Find("DragonflyPoints").GetComponent<PatrolPoints>();
        PositionIndexSet(true);
        while(!PositionSetComplete())
        {
            yield return null;
        }
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
                    OnReturn();
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
        PositionIndexSet(false);

        if(!PositionSetComplete())
        {
            return;
        }

        animator.SetBool(hashAttack, false);
        animator.SetBool(hashTrace, true);
        agent.isStopped = false;

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
                isPosition = false;
            }
        }
    }

    public void PositionIndexSet(bool isRandom)
    {
        var dragonflies = GameObject.FindObjectsByType<DragonflyMove>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.InstanceID
                );

        if (dragonflies != null)
        {
            for (int i = 0; i < dragonflies.Length; i++)
            {
                if (i == 0)
                {
                    idx = isRandom ? Random.Range(0, path.GetWayCount()) : NearPositionIndexSet();
                }

                dragonflies[i].idx = idx;
            }

            isPosition = true;
        }
    }

    private int NearPositionIndexSet()
    {
        float dist = 0;
        int idx = 0;
        for (int i = 0; i < path.GetWayCount(); i++)
        {
            float temp = (path.GetWayPoint(i) - transform.position).sqrMagnitude;
            if (temp > dist)
            {
                dist = temp;
                idx = i;
            }
        }

        return idx;
    }

    private bool PositionSetComplete()
    {
        var dragonflies = GameObject.FindObjectsByType<DragonflyMove>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
                );

        foreach (DragonflyMove dragonfly in dragonflies)
        {
            if (!dragonfly.isPosition)
            {
                return false;
            }
        }

        return true;
    }
}
