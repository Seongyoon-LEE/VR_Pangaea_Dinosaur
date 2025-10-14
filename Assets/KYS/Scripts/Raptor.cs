using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Raptor : MonoBehaviour
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
    public float aroundCheckRadius = 30; // 주변 감지 거리
    private LayerMask raptorLayer;
    private int weaponLevel;
    private int hungryLevel;

    private bool isAttacking = false;
    void Start()
    {
        this.raptorLayer = LayerMask.GetMask("Raptor");
        StartCoroutine(SeenRoutine());
        StartCoroutine(AroundRoutine());
        StartCoroutine(WeaponCheckRoutine());
        StartCoroutine(HungryRoutine());
        StartCoroutine(AttackStartRoutine());
    }

    WaitForSeconds ws = new WaitForSeconds(1);
    IEnumerator SeenRoutine()
    {
        var vis = this.GetComponent<Renderer>();
        while (!isAttacking)
        {
            if (!vis.isVisible)
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
            if (Physics.OverlapSphere(this.transform.position, this.aroundCheckRadius,this.raptorLayer).Length > 3) // 랩터만 체크
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
            if (true)
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
        foreach(var raptor in Physics.OverlapSphere(this.transform.position, this.aroundCheckRadius, this.raptorLayer))
        {
            var rap = raptor.GetComponent<Raptor>();
            if (!rap.isAttacking)
            {
                rap.StartAttack();
            }
        }
        //공격 로직
    }
}
