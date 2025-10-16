using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CryingAngel : Dino
{
    Renderer render;
    public float sensorDist = 100;
    IEnumerator patrol;
    IEnumerator chase;
    private void Start()
    { 
        this.render = GetComponent<Renderer>();
        this.agent = GetComponent<NavMeshAgent>();
        this.agent.speed = speed;
        this.patrol = this.PatrolRoutine();
        this.chase = this.ChaseRoutine();
    }
    
    WaitForSeconds wsForMove = new WaitForSeconds(0.2f);
    IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (this.render.isVisible)
            {
                //어떤 카메라에든 렌더링이 잡히면 멈춤
                this.agent.speed = 0;
                this.agent.angularSpeed = 0;
            }
            else
            {
                //안잡히면 움직임
                this.agent.speed = this.speed;
                this.agent.angularSpeed = this.angularSpeed;
            }
            yield return wsForMove;
        }
    }
    IEnumerator StatusRoutine()
    {
        eStatus tempStatus = this.Status;
        while (true)
        {
            // 상태변화 조건 : 일정 범위 내에 사람이 있는가(벽 무시)
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, sensorDist, LayerMask.GetMask(this.playerStr));
            if (targetsInViewRadius.Length > 0)
            {
                tempStatus = eStatus.Active;
            }
                if (this.Status != tempStatus) // 상태가 변화함
            {
                this.Status = tempStatus;
            }
        }
    }
    public override void Wait()
    {
        //배회
        StopCoroutine(this.chase);
        StartCoroutine(this.patrol);
    }
    IEnumerator PatrolRoutine()
    {
        yield return null; // 순찰 포인트 잡아서 순찰하도록
    }
    public override void Active()
    {
        //추적
        StopCoroutine(this.patrol);
        StartCoroutine(this.chase);
    }
    IEnumerator ChaseRoutine()
    {
        yield return null; // 타겟으로 잡힌 플레이어 따라가도록
    }
}
