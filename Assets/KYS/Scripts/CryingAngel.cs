using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryingAngel : MonoBehaviour
{
    Renderer render;
    private void Start()
    { 
        this.render = GetComponent<Renderer>();
    }

    private void Update()
    {
        
    }
    WaitForSeconds wsForMove = new WaitForSeconds(0.2f);
    IEnumerator moveRoutine()
    {
        while (true)
        {
            if (this.render.isVisible)
            {
                //어떤 카메라에든 렌더링이 잡히면 멈춤
            }
            else
            {
                //안잡히면 움직임
            }
            yield return wsForMove;
        }
    }
}
