using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : ProjectileBasic
{
    
    public Vector3 mEnemyInitPos;

    public EnemyBehaviour mEnemy;
    

    // Start is called before the first frame update
    void Start()
    {
        m_ObjectCollider = GetComponent<Collider>();
        mInitPos = gameObject.transform.position;
        mEnemyInitPos = mEnemy.transform.position;
        //mDestPos = (Vector3.Distance(mEnemy.transform.position, mInitPos)/mSpeed)*(mEnemy.transform.forward*mEnemy.mSpeed) + mInitPos ;
        Debug.Log("Run!"+mInitPos+ " "+mDestPos);
    }


    public new void Fly()
    {
        float lerpFactor = mDeltaTime / mTime;

        Vector3 pos = Vector3.Lerp(mInitPos, mEnemy.transform.position, lerpFactor);// 
        transform.position = pos;
        transform.position = new Vector3(transform.position.x, mAnimationCurve.Evaluate(lerpFactor) * 4, transform.position.z);

        mDeltaTime += Time.deltaTime;
    }
    // Update is called once per frame
    void Update()
    {

        if (mDeltaTime < mTime)
        {
            Fly();
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
