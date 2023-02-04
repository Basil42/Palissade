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
        // m_ObjectCollider.isTrigger = false;
        mDeltaTime = 0;
        mInitPos = gameObject.transform.position;
        mEnemyInitPos = mEnemy.transform.position;
        mDestPos = (Vector3.Distance(mEnemy.transform.position, mInitPos)/mSpeed)*(mEnemy.transform.forward*mEnemy.mSpeed) + mInitPos ;
        Debug.Log("Run!"+mInitPos+ " "+mDestPos);
    
    
    }

    private void OnTriggerEnter(Collider other){

        other.GetComponentInParent<EnemyBehaviour>().mLife -= 1;
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
            // float lerpFactor = Vector3.Distance(mInitPos ,transform.position) / Vector3.Distance(mInitPos,mEnemyInitPos);
            float lerpFactor = mDeltaTime/mTime; 
            float height = 2;
            //Vector3 pos = Vector3.Lerp(mInitPos, mDestPos, mAnimationCurve.Evaluate(lerpFactor));//

            Vector3 pos = Vector3.Lerp(mInitPos, mEnemy.transform.position, lerpFactor);// 

            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(mEnemy.transform.position - transform.position), mSpeed * Time.deltaTime);

            // Debug.Log("lerpFactor "+lerpFactor + " pos "+ pos + "Norm = " + Vector3.Normalize(mEnemy.transform.position - transform.position));
            // transform.position = Vector3.Normalize(mDestPos-mInitPos)*mSpeed*mDeltaTime+mInitPos +new Vector3(0, lerpFactor*height,0);
            // transform.position = Vector3.Normalize(mEnemy.transform.position - transform.position) * mSpeed * mDeltaTime + mInitPos;// +new Vector3(0, lerpFactor,0);

            transform.position = pos;
            // transform.position += transform.forward * Time.deltaTime * mSpeed;
            /*transform.position.y += lerpFactor;*/
            transform.position = new Vector3(transform.position.x, mAnimationCurve.Evaluate(lerpFactor)*4, transform.position.z);
            // transform.position += Vector3.Normalize(mEnemy.transform.position - transform.position) * mSpeed;// +new Vector3(0, lerpFactor,0);

            mDeltaTime += Time.deltaTime;   


        }else
        {
            Destroy(gameObject);
        }
        
    }
}
