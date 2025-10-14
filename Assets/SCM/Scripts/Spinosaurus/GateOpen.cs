using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpen : MonoBehaviour
{
    Animator animator;
    WaitForSeconds openSeconds;
    bool isOpen = false;
    SpinosaurusCtrl spinosaurus;
    void Start()
    {
        animator = GetComponent<Animator>();
        openSeconds = new WaitForSeconds(180f);
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
            // 스피노 등장 메서드 호출
            spinosaurus.DinoAppeared();
        }


    }
}
