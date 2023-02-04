using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAI : TowerBasic
{

    public float mRayShoot;
    public float mSpeedShoot;
    public float mCurrentTime;

    // public TowerAI(){
    //     mRayShoot = 10;
    //     mSpeed = 5;
    // }



    void Shoot(){
        if (mEnemies.Count != 0)
        {
            SortEnemy();
            Vector3 m = transform.position;
            m += Vector3.up * 2.0f;
            ProjectileBehaviour ball = Instantiate(mProjectile, m,  transform.rotation );
            ball.mEnemy = mEnemies[0];
            // ball.gameObject.GetComponent<Rigidbody>().velocity = (Vector3.up + Vector3.forward) * ball.mSpeed;
            // ball.gameObject.transform.position = transform.TranformPoint(Vector3.forward * 1.5f);
            // ball.gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3 (0, ball.mSpeed,0));
            // ball.gameObject.transform.rotation = Quaternion.LookRotation(mEnemies[0].gameObject.transform.position - transform.position, Vector3.up);
            // ball.mDestination = mEnemies[0].gameObject.transform.position;
            Damage(mEnemies[0]);
        }

    }

    void projectileLauncher(EnemyBehaviour enemy){

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
