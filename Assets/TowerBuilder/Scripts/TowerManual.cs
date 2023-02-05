using System.Collections;
using System.Collections.Generic;
//using UnityEngine.InputSystem;
using UnityEngine;

public class TowerManual : TowerBasic
{
    private new Camera camera;
    private Vector3 mMousePosition;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    void Shoot()
    {
        Vector3 m = transform.position;
        //m += Vector3.up * 2.0f;
        ProjectileBasic ball = (ProjectileBasic) Instantiate(mProjectile, m, transform.rotation);
        ball.mDestPos = mMousePosition;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
                //Debug.Log("Left Click");
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                    //Debug.Log($"{hit.collider.name} Detected", hit.collider.gameObject);
               
                mMousePosition = hit.point;
                
                Shoot();
            }

            //RandomColor();
        }
    }
}
