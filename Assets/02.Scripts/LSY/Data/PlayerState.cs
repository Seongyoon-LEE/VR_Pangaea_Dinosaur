using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Hand,       // 맨손 (기본 상태)
    Inventory,  // 인벤토리 UI를 보고 있는 상태
    Revolver,   // 리볼버를 든 상태
    Lighter,    // 라이터를 든 상태
    Camera,     // 카메라를 든 상태
    Hiding,     // 숨는 중이거나 숨어있는 상태
    CameraView
}
