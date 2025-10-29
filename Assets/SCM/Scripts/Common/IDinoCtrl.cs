using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDinoCtrl
{
    public void FindOut(Transform tr, bool isHide);
    public void PlayerLeave(bool isHide);
    IEnumerator UpdateCurrentStatus();

    void OnPatrol();
    void OnTrace();
    void OnAttack();
    void OnIdle();
}
