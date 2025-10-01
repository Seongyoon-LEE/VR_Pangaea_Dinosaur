using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonflyMove : MonoBehaviour
{
    private readonly int hashTrace = Animator.StringToHash("isTrace");
    private readonly int hashAttack = Animator.StringToHash("isAttack");
    private readonly string torchTag = "TORCH";
    private enum Status
    {
        None, TRACE, ATTACK, ESCAPE
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
    bool isEscape = false;
    void Start()
    {
        playerTr = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ws = new WaitForSeconds(0.5f);
        escapeSeconds = new WaitForSeconds(3f);

        StartCoroutine(StatusCheck());
        StartCoroutine(FindPlayer());
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
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(torchTag))
        {
            isEscape = true;
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

    IEnumerator FindPlayer()
    {
        while(true)
        {
            yield return ws;

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

            if (isEscape)
            {
                status = Status.ESCAPE;
                break;
            }
        }
    }
}
