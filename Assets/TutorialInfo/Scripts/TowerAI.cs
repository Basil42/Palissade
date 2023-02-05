using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAI : TowerBasic
{

    public float mRayShoot;
    public float mSpeedShoot;
    public float mCurrentTime;


    void Shoot(){
        if (mEnemies.Count != 0)
        {
            SortEnemy();
            Vector3 m = transform.position;
            m += Vector3.up * 2.0f;
            ProjectileBehaviour ball = (ProjectileBehaviour) Instantiate(mProjectile, m,  transform.rotation );
            ball.mEnemy = mEnemies[0];
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ManageDeadEnemy();
        if (mEnemies.Count != 0){
            if (mCurrentTime < 0){
                Shoot();
                mCurrentTime = mSpeedShoot;
            }else{
                mCurrentTime -= Time.deltaTime; 
            }
        }


        // Shoot();
    }
}
