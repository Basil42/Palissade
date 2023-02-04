using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float mSpeed;
    public float mTime;
    public float mDeltaTime;
    public Vector3 mInitPos;
    public Vector3 mDestPos;
    public AnimationCurve mAnimationCurve;
    public Vector3 mEnemyInitPos;

    public EnemyBehaviour mEnemy;
    Collider m_ObjectCollider;
    

    // Start is called before the first frame update
    void Start()
    {
        m_ObjectCollider = GetComponent<Collider>();
        //Here the GameObject's Collider is not a trigger
        m_ObjectCollider.isTrigger = false;
        mDeltaTime = 0;
        mInitPos = gameObject.transform.position;
        mEnemyInitPos = mEnemy.transform.position;
        mDestPos = (Vector3.Distance(mEnemy.transform.position, mInitPos)/mSpeed)*(mEnemy.transform.forward*mEnemy.mSpeed) + mInitPos ;
        Debug.Log("Run!"+mInitPos+ " "+mDestPos);
    
    
    }

    private void OnTriggerEnter(Collider other){
        other.GetComponent<EnemyBehaviour>().mLife -= 1;
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
        // mAnimationCurve
        // transform.Translate(0, mSpeed* Time.deltaTime,0 );
        if (mDeltaTime < mTime)
        {
            // float lerpFactor = Vector3.Distance(mEnemyInitPos,mEnemy.transform.position) / Vector3.Distance(mEnemyInitPos,mDestPos);
            float lerpFactor = Vector3.Distance(transform.position,mDestPos) / Vector3.Distance(mEnemyInitPos,mDestPos);
            float height = 2;
            Vector3 pos = Vector3.Lerp(mInitPos, mDestPos, mAnimationCurve.Evaluate(lerpFactor));// 
            Debug.Log("lerpFactor "+lerpFactor+"pos "+ pos);
            // transform.position = Vector3.Normalize(mDestPos-mInitPos)*mSpeed*mDeltaTime+mInitPos +new Vector3(0, lerpFactor*height,0);
            transform.position = Vector3.Normalize(mEnemy.transform.position - transform.position)*mSpeed*mDeltaTime+mInitPos +new Vector3(0, lerpFactor*height,0);

            mDeltaTime += Time.deltaTime;   


        }
        // if (m_ObjectCollider.isTrigger)
        // {
        //     Destroy(this);
        // }
        // }else
        // {
        //     Destroy();
        // }
        
    }
}
