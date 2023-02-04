using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public int mLife;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool isDead(){
        return mLife <= 0;
    }

    void Show(){
        // Debug.Log("Je suis a la position" + );
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (isDead())
    //     {
            
    //     }
    // }
}
