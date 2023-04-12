using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAI : TowerBasic
{
    public float mRayShoot;
    public float mSpeedShoot;
    private float mCurrentTime;

    public ShipBehavior target;

    private void Awake()
    {
        GameManager.OnEnterAttackMode += () =>
        {
            this.enabled = true;
        };
        GameManager.OnExitAttackMode += () =>
        {
            this.enabled = false;
        };
    }

    void Shoot()
    {
        if (target != null)
        {
            //SortEnemy();
            Vector3 m = transform.position;
            m += Vector3.up * 2.0f;
            ProjectileBasic ball = Instantiate(mProjectile, m, transform.rotation);
            ball.mDestPos = ComputeDestination(target);
        }
    }

    private Vector3 ComputeDestination(ShipBehavior shipBehavior)
    {
        var targetPos = target.transform.position;
        return targetPos + (target.destination - targetPos) * (target.speed * mProjectile.mFlyTimer);//misses on ship that are almost to their destination, should be ok
    }

    void Update()
    {
        if (target != null && !target.isDestroyed)
        {
            if (mCurrentTime < 0)
            {
                Shoot();
                mCurrentTime = mSpeedShoot;
            }
            else
            {
                mCurrentTime -= Time.deltaTime;
            }
        }
        else
        {
            // Truc dégueu
            ShipBehavior[] targets = FindObjectsOfType<ShipBehavior>();
            foreach(ShipBehavior tmpTarget in targets) 
            {
                if(!tmpTarget.isDestroyed)
                {
                    target = tmpTarget;
                    break;
                }
            }

           // target = FindObjectOfType<ShipBehavior>();
        }
    }

    /*public void SortEnemy()
    {
        target.Sort((enemy, enemy2) =>
        {
            if (enemy == null && enemy2 == null) return 0;
            else if (enemy == null) return -1;
            else if (enemy2 == null) return 1;
            else return (Vector3.Distance(transform.position, enemy.gameObject.transform.position) < Vector3.Distance(transform.position, enemy2.gameObject.transform.position)) ? -1 : 1;
        });
        // Vector3.Distance(transform.position, obj.transform.position)
    }*/
}
