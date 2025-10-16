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
    protected NavMeshAgent agent;
    protected readonly string playerStr = "Player";
    private eStatus _status;
    public eStatus Status
    {
        get
        {
            return _status;
        }
        set
        {
            this._status = value;
            if(value == eStatus.Active)
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
}
