using System.Collections;
using System.Collections.Generic;
//using UnityEngine.InputSystem;
using UnityEngine;

public class TowerManual : TowerBasic
{
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        ManageDeadEnemy();
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left Click");
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log($"{hit.collider.name} Detected",
                    hit.collider.gameObject);
            }
            //RandomColor();
        }
    }
}
