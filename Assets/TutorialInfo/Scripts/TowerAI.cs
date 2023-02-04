using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAI : TowerBasic
{

    float mRayShoot;
    float mSpeedShoot;
    float mCurrentTime;

    // public TowerAI(){
    //     mRayShoot = 10;
    //     mSpeed = 5;
    // }



    void Shoot(){
        SortEnemy();
        // mEnemies[0];
        Debug.Log("Je tire sur "+ mEnemies[0].gameObject.name+" "+ mEnemies[0].gameObject.transform.position);
        Damage(mEnemies[0]);
        Debug.Log(mEnemies[0].gameObject.name + " life : "+ mEnemies[0].mLife);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mCurrentTime < 0){
            Shoot();
            mCurrentTime = mSpeedShoot;
        }else{
            mCurrentTime -= Time.deltaTime; 
        }
        // Shoot();
    }
}
