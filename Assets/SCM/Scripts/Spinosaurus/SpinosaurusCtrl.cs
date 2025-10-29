using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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
    Rigidbody rb;
    private Transform playerTr;
    PatrolPoints path;
    int idx = 0;
    public float runSpeed = 5f;
    float rotSpeed = 10f;
    private float upTime = 30f;
    private float downTime = 5f;
    private float topY = 1f;
    private float bottomY = -11f;
    private bool isEnable = false;
    bool isHide = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        rb = GetComponent<Rigidbody>();
        ws = new WaitForSeconds(0.3f);
        path = GameObject.Find("SpinoPoints").GetComponent<PatrolPoints>(); // 이름 변경
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

    public IEnumerator DinoUpAndDown(bool isUp)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos;
        endPos.y = isUp ? topY : bottomY;

        float moveTime = isUp ? upTime : downTime;

        float startTime = Time.time;
        while(Time.time <= startTime + moveTime)
        {
            float y = (Time.time - startTime) / moveTime;
            transform.position = Vector3.Lerp(startPos, endPos, y);

            yield return null;
        }

        transform.position = endPos;
        DinoAppeared();
    }
    public void DinoAppeared()
    {
        isEnable = true;
        agent.enabled = true;
        rb.useGravity = true;
        StartCoroutine(UpdateCurrentStatus());
        status = Status.PATROL;
    }
    public void FindOut(Transform tr, bool isHide)
    {
        if (!isEnable) return;
        status = Status.TRACE;
        playerTr = tr;
        this.isHide = isHide;
    }

    public void PlayerLeave(bool isHide)
    {
        if (PlayerStateManager.Instance.CurState != PlayerState.Hiding) return;

        this.isHide = isHide;
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
        if (!isHide)
        {
            status = Status.TRACE;
            return;
        }

        if (Vector3.Distance(path.FlattenY(playerTr.position), path.FlattenY(transform.position)) < 6f)
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
        if (Vector3.Distance(path.FlattenY(playerTr.position), path.FlattenY(transform.position)) > 6f)
        {
            status = Status.TRACE;
            return;
        }

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
