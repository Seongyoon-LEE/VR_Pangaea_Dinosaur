using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class DragonflyMove : MonoBehaviour, IDinoCtrl
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
    float attackRange = 4f;
    float escapeRange = 90f;
    float scatterRange = 5f;
    float rotSpeed = 10;
    float attackSpeed = 3f;
    float nextAttackTime = 0f;
    bool isTorch = false;
    public int idx;
    float stoppingDistance = 3f;
    public bool isPosition = false;
    public bool isChasingPlayer = false;
    bool isFocus = false;
    PatrolPoints path;
    IEnumerator Start()
    {
        playerTr = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ws = new WaitForSeconds(0.1f);
        escapeSeconds = new WaitForSeconds(3f);

        path = GameObject.Find("DragonFlyPoints").GetComponent<PatrolPoints>();

        agent.avoidancePriority = Random.Range(50, 80);
        agent.enabled = false;
        PointIndexSet(true);
        while(!PointSetComplete())
        {
            yield return null;
        }
        RespawnPosition(path.GetWayPoint(idx));
        //transform.position = path.GetWayPoint(idx);

        StartCoroutine(UpdateCurrentStatus());
        StartCoroutine(FindPlayer());
    }
    private void Update()
    {
        if (isFocus)
        {
            Vector3 taget = (playerTr.position - transform.position).normalized;
            

            Quaternion rot = Quaternion.LookRotation(taget);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotSpeed);
        }
    }
    public IEnumerator UpdateCurrentStatus()
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
                    OnIdle();
                    break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(torchTag))
        {
            isTorch = true;
        }
    }

    public void OnIdle()
    {
        isFocus = false;
        animator.SetBool(hashTrace, false);
        animator.SetBool(hashAttack, false);
        agent.isStopped = true;
    }

    public void OnTrace()
    {
        print("추격");
        isFocus = true;
        animator.SetBool(hashTrace, true);
        animator.SetBool(hashAttack, false);

        agent.isStopped = false;
        if (agent.velocity.sqrMagnitude <= 0.1f)
        {
            // 플레이어 주변의 랜덤한 목표 지점 계산
            Vector3 randomPoint = playerTr.position + Random.insideUnitSphere * scatterRange;
            NavMeshHit hit;

            // NavMesh 위에서 유효한 지점 찾기
            if (NavMesh.SamplePosition(randomPoint, out hit, scatterRange, NavMesh.AllAreas))
            {
                // 새로운 목표 지점으로 설정하여 몬스터 사이를 비집고 들어가거나 맴돌도록 유도
                agent.SetDestination(hit.position);
            }
        }
        // Case 2: 플레이어에게 접근 중일 때
        else
        {
            // 플레이어의 현재 위치를 목표로 설정
            agent.SetDestination(playerTr.position);
        }

        //agent.destination = playerTr.position;
    }

    public void OnAttack()
    {
        if (Time.time < nextAttackTime) return;

        animator.SetBool(hashAttack, true);
        agent.isStopped = true;

        nextAttackTime = Time.time + attackSpeed;
    }

    // 플레이어 반대 방향으로 도망가는 로직
    IEnumerator OnEscape()
    {
        isFocus = false;
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
        PointIndexSet(false);
        isFocus = false;
        if (!PointSetComplete())
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

            if (isTorch)
            {
                // 회피
                //status = Status.ESCAPE;
                //break;

                // 복귀
                status = Status.RETURN;
                isPosition = false;
                continue;
            }


            float dist = (playerTr.position - transform.position).sqrMagnitude;

            if (dist < attackRange)
            {
                status = Status.ATTACK;
                isChasingPlayer = false;
            }
            else if (dist < traceRange)
            {
                status = Status.TRACE;
            }
            else
            {
                if (isChasingPlayer) continue;
                status = Status.None;
            }

            
        }
    }

    // 잠자리의 패트롤 포인트 설정
    public void PointIndexSet(bool isRandom)
    {
        // 활성화된 잠자리를 InstanceID순으로 불러오기
        var dragonflies = GameObject.FindObjectsByType<DragonflyMove>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.InstanceID
                );

        if (dragonflies != null)
        {
            for (int i = 0; i < dragonflies.Length; i++)
            {
                if (i == 0 && !dragonflies[i].isPosition) // 첫번째이고 아직 설정이 되어있지 않으면
                {
                    // 랜덤 or 멀리 있는 포인트
                    idx = isRandom ? Random.Range(0, path.GetWayCount()) : FarPointIndexSet();
                }

                if (!dragonflies[i].isPosition) // 미설정시 설정
                {
                    dragonflies[i].idx = idx; // 위치값
                    dragonflies[i].isPosition = true; // 설정 완료
                }
            }
        }
    }

    // 멀리 있는 포인트 반환
    private int FarPointIndexSet()
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

    // 모든 잠자리가 위치 설정이 되었는지 체크
    private bool PointSetComplete()
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

    private void RespawnPosition(Vector3 point)
    {
        float radius = 0f;
        float maxOffsetRange = 1f;
        var col = GetComponent<CapsuleCollider>();
        if (col != null)
        {
            radius = col.radius * 1.05f;
        }

        while(true)
        {
            Vector3 randomOffset = new Vector3(
                    Random.Range(-maxOffsetRange, maxOffsetRange),
                    0.3f,
                    Random.Range(-maxOffsetRange, maxOffsetRange)
                );

            Vector3 attemptedPosition = point + randomOffset;

            // Physics.CheckSphere를 사용하여 겹침 확인
            // CheckSphere가 false를 반환해야 겹침이 없는 것입니다.
            if (!Physics.CheckSphere(attemptedPosition, radius, 1 << this.gameObject.layer))
            {
                // 겹침이 없다면 위치를 확정하고 루프 종료
                transform.position = attemptedPosition;
                break;
            }
        }
        print("배치 완료");
        agent.enabled = true;
    }

    public void FindOut(Transform tr)
    {
        throw new System.NotImplementedException();
    }
    public void ClearSight()
    {
        throw new System.NotImplementedException();
    }
    public void OnPatrol()
    {
        throw new System.NotImplementedException();
    }

    
}
