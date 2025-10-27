using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parasau : Dino
{
    private void Start()
    {
        this.patrol = this.PatrolRoutine();
        this.chase = this.ChaseRoutine();

        this.Status = eStatus.Wait;
    }
    WaitForSeconds wsForSleep = new WaitForSeconds(0.2f);
    IEnumerator SleepRoutine()
    {
        while (true)
        {
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, sensorDist, LayerMask.GetMask(this.playerStr));
            if(targetsInViewRadius.Length > 0)
            {
                bool check = false;
                foreach(Collider target in targetsInViewRadius)
                {
                    //target의 상태값이 뛰는거라면(플레이어 만들어진거 보고 제작)
                    if (target.GetComponent<KYS_Player_Status>().status == ePlayerStatus.Running)
                    {
                        check = true;
                        break;
                    }
                }
                if (check)
                    break;
            }
            yield return wsForSleep;
        }
        //반복문이 끝남 = 누군가 뛰어서 break로 나옴
        //울어서 주변 공룡 전부 활성화
        Active();
    }

    public override void Wait()
    {
        //잠
        StartCoroutine(this.SleepRoutine());
    }

    public override void Active()
    {
        //울어서 주변 공룡 전부 활성화
        var saurs = Physics.OverlapSphere(transform.position, sensorDist, LayerMask.GetMask("DINO"));
        foreach(var saur in saurs)
        {
            saur.GetComponent<Dino>().Status = eStatus.Active;
        }
    }
}
