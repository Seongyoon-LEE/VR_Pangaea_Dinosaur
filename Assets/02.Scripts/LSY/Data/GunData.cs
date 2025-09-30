using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� �Ӽ�
[CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/GunData", order = 1)]
public class GunData : ScriptableObject
{
    [Header("�Ѿ� ����")]
    public int maxAmmo = 100;       // �ִ� ź��
    public int curAmmo;          // ���� ź��

    [Header("�߻� ����")]
    public int damage = 10;     // �Ѿ� ������
    public int range = 100;     // ��Ÿ�
    public float fireRate = 0.5f; // ���� �ӵ� (�ʴ� �߻� Ƚ��)

    [Header("�ݵ� ����")]
    public float recoilForce = 4.0f; // �ݵ� ����
    public float recoilDuration = 0.1f; // �ݵ� ���� �ð�

    [Header("������ ����")]
    public float reloadTime = 1.5f; // ������ �ð�

    [Header("����")]
    public AudioClip shootSound; // �߻� ����
    public AudioClip reloadSound; // ������ ����
    public AudioClip emptySound; // ź�� ���� ����
}
