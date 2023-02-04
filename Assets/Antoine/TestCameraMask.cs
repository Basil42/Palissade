using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraMask : MonoBehaviour
{
    public Camera cam1;

    public LayerMask mode2D;
    public LayerMask mode3D;

    private bool isMode2D;

    private void Start()
    {
        cam1.cullingMask = mode2D;
        isMode2D= true;
    }

    public void ChangeMode()
    {
        if (isMode2D)
        {
            cam1.cullingMask = mode3D;
            isMode2D = false;
        }
        else
        {
            cam1.cullingMask = mode2D;
            isMode2D = true;
        }
    }
}
