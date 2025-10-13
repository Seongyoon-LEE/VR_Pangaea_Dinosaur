using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraControl : MonoBehaviour
{
    public bool test;
    public Camera cam;
    public LayerMask normalLayerMask;
    public LayerMask infraredLayerMask;
    void Update()
    {
        if (test)
        {
            cam.cullingMask = normalLayerMask;
        }
        else
        {
            cam.cullingMask = infraredLayerMask;
        }
        
    }
}
