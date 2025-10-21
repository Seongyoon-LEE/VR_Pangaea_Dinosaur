using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpen : MonoBehaviour
{
    Animator animator;
    WaitForSeconds openSeconds;
    WaitForSeconds ws;
    bool isOpen = false;
    SpinosaurusCtrl spinosaurus;
    float curTime = 0f;
    void Start()
    {
        animator = GetComponent<Animator>();
        openSeconds = new WaitForSeconds(180f);
        ws = new WaitForSeconds(0.3f);
        spinosaurus = GetComponent<SpinosaurusCtrl>(); // 위치 변경
        StartCoroutine(GateOepn());
    }

    IEnumerator GateOepn()
    {
        while(true)
        {
            yield return openSeconds;

            if (isOpen) continue;
            if (Random.value <= 0.3f) continue;

            isOpen = true;
            // 오픈 애니메이션
            // 사이렌 소리
            // 스피노 등장
            StartCoroutine(RiseDinoWater());
        }
    }

    IEnumerator RiseDinoWater()
    {
        while(true)
        {
            curTime += Time.deltaTime;
            yield return ws;

            // 서서히 올라오게

            // 스피노 물 밖 등장
            if (curTime - Time.deltaTime >= 30f)
            {
                spinosaurus.DinoAppeared();
            }
        }
    }

    public void GateClose()
    {
        isOpen = false;
        curTime = 0f;
        // 스피노 위치 초기화
        // 철창 닫는 애니메이션
        // 사이렌 종료
    }
}
