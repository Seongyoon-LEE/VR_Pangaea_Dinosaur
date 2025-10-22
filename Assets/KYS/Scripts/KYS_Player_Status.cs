using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ePlayerStatus
{
    Normal = 0, Lighter = 1, TryHide = 2
}
public class KYS_Player_Status : MonoBehaviour
{
    /*
     필요한 기능 : 
    라이터 키기 : 몇 초 뒤 라이터 활성화 -> 
    숨기 : 숨는 중 상태가 되었다가, 숨는 중이 끝나면 숨은 layer로 변경
    데미지 입기 : 맞기
    총쏘기 : 총쏘기
    적외선 시야 사용 : 카메라 마스크 설정 건드리기
     */
    public ePlayerStatus status = ePlayerStatus.Normal;
    public void ChangeStatus(ePlayerStatus newStatus)
    {
        this.status = newStatus;
    }
}
