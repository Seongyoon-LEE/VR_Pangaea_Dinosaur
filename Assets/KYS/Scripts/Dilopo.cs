using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.XR.Interaction;
/*
플레이어 위치 값을 멀리서부터 알 수 있다. 딜로포사우루스는 플레이어 이동속도에 0.5배의 속도로 쫒아온다.
플레이어에게 닿을 시 공격하며 라이터를 들 시 멀리 도망간다.
총을 맞을 시 3초간 기절한다. 
*/
public class Dilopo : Dino
{
    private Transform playerTr;

    bool isStun = false;
    // Start is called before the first frame update
    void Start()
    {
        this.playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        //this.moveSpeed = this.playerTr.GetComponent<Player>().moveSpeed / 2;
        this.agent = GetComponent<NavMeshAgent>();
        this.agent.speed = speed;
        this.patrol = this.PatrolRoutine();
        this.chase = this.ChaseRoutine();
    }
    WaitForSeconds wsForStun = new WaitForSeconds(3);
    IEnumerator MoveRoutine(Transform player)
    {
        var playerStatus = player.gameObject.GetComponent<KYS_Player_Status>();
        //매개변수로 player를 받아와서 상태 추적, transform말고 상태값 들어있는 스크립트
        while (true)
        {
            if (!this.isStun) // 총맞아서 기절한게 아닌 동안
            {
                if (playerStatus.status != ePlayerStatus.Lighter)// player의 상태값이 라이터가 아닌동안(true에서 바꾸기)
                {
                    //navMesh를 이용해서 플레이어쪽으로 이동
                    yield return wsForMove;//매 프레임마다가 아닌 0.2초마다 갱신
                }
                else // 라이터일때
                {
                    //도망감?
                    yield return wsForMove;
                }
            }
            else // 총맞아서 기절
            {
                //기절 해제
                this.isStun = false;
                //안움직임
                yield return wsForStun;
            }
            
        }
    }

    IEnumerator StatusRoutine() // 활성화로만 변화
    {
        eStatus tempStatus = eStatus.Wait;
        while (true)
        {
            // 상태변화 조건 : 일정 범위 내에 사람이 있는가(벽 무시)
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, sensorDist, LayerMask.GetMask(this.playerStr));
            if (targetsInViewRadius.Length > 0)
            {
                this.target = targetsInViewRadius[0].transform;
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
