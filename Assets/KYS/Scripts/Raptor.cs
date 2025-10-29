using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.AI;

public class Raptor : Dino
{
    // 공격 레벨 일정 이상이 됐을 경우 즉시 공격 시작, 한번 공격 시작하면 공격을 취소하지 않음
    // 공격 시작시, 주변 랩터들 전부를 공격 시작 상태로 변경
    // 공격 레벨 조건 : 점수 합 50점 이상이면 공격 시작
    // 1. 플레이어가 랩터를 쳐다보지않을때 : + 10
    // 2. 근처 랩터가 본인 포함 4마리 이상일 때 : +30
    // 3. 플레이어가 무기를 들고 있을 때 : -10
    // 4. 포만감이 100에서 시작해서 초당 1씩 감소한다, 이에 따른 공격 레벨 : -((현재 포만감) - 70)
    private int seenLevel;
    private int aroundLevel;
    private LayerMask raptorLayer;
    private int weaponLevel;
    private int hungryLevel;

    public Renderer body;

    private bool isAttacking = false;
    void Start()
    {
        this.raptorLayer = LayerMask.GetMask("Raptor");
        this.target = GameObject.FindAnyObjectByType<PlayerMovement>().transform;
        StartCoroutine(SeenRoutine());
        StartCoroutine(AroundRoutine());
        StartCoroutine(WeaponCheckRoutine());
        StartCoroutine(HungryRoutine());
        StartCoroutine(AttackStartRoutine());

        this.agent = GetComponent<NavMeshAgent>();
        this.agent.speed = speed;
        this.patrol = this.PatrolRoutine();
        this.chase = this.ChaseRoutine();

        this.Status = eStatus.Wait;
    }

    WaitForSeconds ws = new WaitForSeconds(1);
    IEnumerator SeenRoutine()
    {
        while (!isAttacking)
        {
            if (!body.isVisible)
            {
                seenLevel = 10;
            }
            else
            {
                seenLevel = 0;
            }
            yield return ws;
        }
    }
    IEnumerator AroundRoutine()
    {
        while (!isAttacking)
        {
            if (Physics.OverlapSphere(this.transform.position, this.sensorDist,this.raptorLayer).Length > 3) // 랩터만 체크
            {
                aroundLevel = 30;
            }
            else
            {
                aroundLevel = 0;
            }
            yield return ws;
        }
    }
    IEnumerator WeaponCheckRoutine()
    {
        while (!isAttacking)
        {
            //무기를 들고있는것 체크
            if (this.target.GetComponent<PlayerStateManager>().CurState == PlayerState.Revolver)
            {
                this.weaponLevel = -10;
            }
            else
            {
                this.weaponLevel = 0;
            }
                yield return ws;
        }
        
    }
    IEnumerator HungryRoutine()
    {
        int hungry = 100;
        while (!isAttacking)
        {
            hungry -= 1;
            this.hungryLevel = -(hungry - 70);
            yield return ws;
        }
    }
    IEnumerator AttackStartRoutine()
    {
        while(this.seenLevel + this.aroundLevel + this.weaponLevel + this.hungryLevel < 50)
        {
            yield return ws;
            //공격 안할때의 로직
            Debug.Log(this.seenLevel + this.aroundLevel + this.weaponLevel + this.hungryLevel);
        }
        //공격 조건 활성화
        StartAttack();
    }
    public void StartAttack()
    {
        this.isAttacking = true;
        StopAllCoroutines();
        Debug.Log("공격 시작");
        foreach(var raptor in Physics.OverlapSphere(this.transform.position, this.sensorDist, this.raptorLayer))
        {
            var rap = raptor.GetComponent<Raptor>();
            if (!rap.isAttacking)
            {
                rap.StartAttack();
            }
        }
        //공격 로직
        Active();
    }

    public override void Wait()
    {
        //배회
        StopCoroutine(this.chase);
        StartCoroutine(this.patrol);
    }

    public override void Active()
    {
        //추적
        StopCoroutine(this.patrol);
        StartCoroutine(this.chase);
    }

    new IEnumerator ChaseRoutine() // 렙터는 다시 비활성화로 돌아가지 않음
    {
        bool watchPlayerHide = false;
        // 타겟으로 잡힌 플레이어 따라가도록
        while (true)
        {
            /*
             0.2초마다 시야 체크 -> 시야에 잡혀있다면 agent 목적지 업데이트
            -> 목적지에 도달하고나서 시야에 안잡힌다면 다시 순찰
            만약 시야에 잡혀있는 상태에서 숨는 행위를 한다면 사망
             */
            // 시야 체크법, agent의 목적지쪽에 ray를 쏴서, 사이에 장애물이 없는지를 체크
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float distToTarget = Vector3.Distance(transform.position, target.position);

            // 1. 공격 가능 거리안에 있는지
            if (distToTarget < 0.5f)//0.5는 임시
            {
                //공격(플레이어 만들어진거 보고 제작)
                yield return wsForAttack;
            }
            // 2. 시야 거리 안에 있는지
            if (Physics.Raycast(transform.position, dirToTarget, out RaycastHit hit, sensorDist))
            {
                // Ray가 맞은 것이 타겟인지 확인
                if (((1 << hit.collider.gameObject.layer) & (watchPlayerHide ? LayerMask.GetMask(this.playerStr) : LayerMask.GetMask(this.playerStr, this.playerHideStr))) != 0)
                {
                    // 타겟이 직접 보임
                    this.agent.destination = this.target.position;
                    //타겟이 숨는 중인지 확인(플레이어 만들어진거 보고 제작)
                    //숨는 중이라면 watchPlayerHide를 true로 변경
                    if (this.target.GetComponent<PlayerStateManager>().CurState == PlayerState.Hiding)
                    {
                        watchPlayerHide = true;
                    }
                    else if (this.target.gameObject.layer == LayerMask.GetMask(this.playerStr))
                    {
                        watchPlayerHide = false;
                    }

                    yield return wsForMove;
                    continue;
                }
                /*else
                {
                    // 중간에 장애물에 막힘
                }*/
            }
            //위 조건 어디에도 안걸렸다면 걸릴때까지 다시
            yield return wsForMove;
        }
    }
}
