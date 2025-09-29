using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager s_Instance;
    public bool isMute = false;
    public float playerVolume = 1.0f;

    private void Awake()
    {
        s_Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SetPlayerVolum(float volume)
    {
        playerVolume = volume;
    }
    public GameObject PlaySfx(Vector3 pos, AudioClip clip, bool isLooped)
    {
        if (isMute) return null;
        GameObject soundObject = new GameObject("-Sound SFX-");
        soundObject.transform.position = pos;
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = isLooped;
        audioSource.minDistance = 20f; // �ּ� �Ÿ� ����
        audioSource.maxDistance = 100f; // �ִ� �Ÿ� ����
        audioSource.volume = playerVolume; // �÷��̾� ���� ����
        audioSource.Play();
        audioSource.mute = isMute; // ���Ұ� ����

        if (!isLooped)
            Destroy(soundObject, clip.length);
        return soundObject;
    }
}