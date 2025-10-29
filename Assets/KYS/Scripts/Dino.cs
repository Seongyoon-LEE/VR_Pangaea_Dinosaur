using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
/*
 공룡에 필요한 내용
1. 타겟쪽으로 움직이는 내용(navMesh 및 그걸 활용하는 함수, 매개변수로 Vector3)
2. 움직이는 속도(float)
 */
public enum eStatus
{
    Wait, Active
}
public abstract class Dino : MonoBehaviour
{
    public float speed;
    public float angularSpeed;
    public PatrolPointsControl patrolPoints;
    protected NavMeshAgent agent;
    protected readonly string playerStr = "PLAYER";
    protected readonly string playerHideStr = "PlayerHide";
    private eStatus _status;

    protected IEnumerator patrol;
    protected IEnumerator chase;
    protected IEnumerator statusCheck;
    protected Transform target;
    public float sensorDist = 100;
    public eStatus Status
    {
        get
        {
            return _status;
        }
        set
        {
            this._status = value;
            if (value == eStatus.Active)
            {
                Active();
            }
            else
            {
                Wait();
            }
        }
    }
    
    public void Move(Vector3 pos) // 좌표 이동
    {
        this.agent.isStopped = false;
        this.agent.Move(pos);
    }
    public void Stop()
    {
        this.agent.isStopped = true;
    }
    public abstract void Wait();
    public abstract void Active();
    protected WaitForSeconds wsForAttack = new WaitForSeconds(3);
    protected WaitForSeconds wsForMove = new WaitForSeconds(0.2f);
    protected IEnumerator PatrolRoutine()
    {
        this.agent.SetDestination(this.patrolPoints.GetNextPoint());
        while (true)
        {
            Debug.Log(Vector3.Distance(this.transform.position, this.patrolPoints.GetPoint()));
            if(Vector3.Distance(this.transform.position, this.patrolPoints.GetPoint()) < 10f)
            {
                this.agent.SetDestination(this.patrolPoints.GetNextPoint());
            }
            yield return wsForMove;
        }
    }
    protected IEnumerator ChaseRoutine()
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
            if (Vector3.Distance(this.transform.position, this.agent.destination) < 2f)
            {
                //목적지에 도착함(마지막으로 본 장소)
                //위에서 시야에 보이는지 이미 체크함 / 보였다면 continue해서 코루틴 처음으로 감
                //즉 여기에 왔으면 안보이고, 목적지에도 도착한거니 다시 순찰
                this.target = null;
                this.Status = eStatus.Wait;
                break;
            }
            //위 조건 어디에도 안걸렸다면 걸릴때까지 다시
            yield return wsForMove;

        }
    }
}
