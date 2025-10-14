using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*
플레이어 위치 값을 멀리서부터 알 수 있다. 딜로포사우루스는 플레이어 이동속도에 0.5배의 속도로 쫒아온다.
플레이어에게 닿을 시 공격하며 라이터를 들 시 멀리 도망간다.
총을 맞을 시 3초간 기절한다. 
*/
public class Dilopo : MonoBehaviour
{
    private Transform playerTr;
    private float moveSpeed = 0;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        this.playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        //this.moveSpeed = this.playerTr.GetComponent<Player>().moveSpeed / 2;
        this.agent = GetComponent<NavMeshAgent>();
        this.agent.speed = moveSpeed;
    }
    WaitForSeconds wsForMove = new WaitForSeconds(0.2f);
    WaitForSeconds wsForStun = new WaitForSeconds(3);
    IEnumerator MoveRoutine(Transform player)
    {
        //매개변수로 player를 받아와서 상태 추적, transform말고 상태값 들어있는 스크립트
        while (true)
        {
            if (true) // 총맞아서 기절한게 아닌 동안
            {
                if (true)// player의 상태값이 라이터가 아닌동안(true에서 바꾸기)
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
            else // 촣맞아서 기절
            {
                //안움직임
                yield return wsForStun;
            }
            
        }
    }
}
