using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager Instance;

    public PlayerState CurState { get; private set; }

    public event Action<PlayerState> OnStateChanged;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 게임 시작시 맨손
        
    }
    public void ChangeState(PlayerState newState)
    {
        // 이미 같은 상태라면 아무것도 안함 (최적화)
        if (CurState == newState) return;

        CurState = newState;
        print($"[PlayerStateManager] State Changed to: {CurState}");

        OnStateChanged.Invoke(newState);
    }
}
