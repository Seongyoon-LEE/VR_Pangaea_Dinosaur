using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CryingAngel : Dino
{
    Renderer render;
    private void Start()
    {
        this.render = GetComponent<Renderer>();
        this.agent = GetComponent<NavMeshAgent>();
        this.agent.speed = speed;
        this.target = null;

        this.patrol = this.PatrolRoutine();
        this.chase = this.ChaseRoutine();
        this.statusCheck = this.StatusRoutine();

        this.Status = eStatus.Wait;
    }
    
    
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
    IEnumerator StatusRoutine() // 활성화로만 변화
    {
        while (true)
        {
            yield return null;
            // 상태변화 조건 : 일정 범위 내에 사람이 있는가(벽 무시)
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, sensorDist, LayerMask.GetMask(this.playerStr));
            if (targetsInViewRadius.Length > 0)
            {
                this.target = targetsInViewRadius[0].transform;
                this.Status = eStatus.Active;
                break;
            }
        }
    }
    public override void Wait()
    {
        //배회
        StopCoroutine(this.chase);
        StartCoroutine(this.statusCheck);
        StartCoroutine(this.patrol);
    }
    
    public override void Active()
    {
        //추적
        StopCoroutine(this.patrol);
        StopCoroutine(this.statusCheck);
        StartCoroutine(this.chase);
    }
}
