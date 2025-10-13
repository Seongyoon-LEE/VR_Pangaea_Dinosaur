using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 에셋 생성 속성
[CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/GunData", order = 1)]
public class GunData : ScriptableObject
{
    [Header("총알 정보")]
    public int maxAmmo = 100;       // 최대 탄약
    public int curAmmo;          // 현재 탄약

    [Header("발사 정보")]
    public int damage = 10;     // 총알 데미지
    public int range = 100;     // 사거리
    public float fireRate = 0.5f; // 연사 속도 (초당 발사 횟수)

    [Header("반동 정보")]
    public float recoilForce = 4.0f; // 반동 세기
    public float recoilDuration = 0.1f; // 반동 지속 시간

    [Header("재장전 정보")]
    public float reloadTime = 1.5f; // 재장전 시간

    [Header("사운드")]
    public AudioClip shootSound; // 발사 사운드
    public AudioClip reloadSound; // 재장전 사운드
    public AudioClip emptySound; // 탄약 부족 사운드
}
