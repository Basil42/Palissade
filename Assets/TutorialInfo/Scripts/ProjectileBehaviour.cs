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
        mDeltaTime = 0;
        mInitPos = gameObject.transform.position;
        mEnemyInitPos = mEnemy.transform.position;
        mDestPos = (Vector3.Distance(mEnemy.transform.position, mInitPos)/mSpeed)*(mEnemy.transform.forward*mEnemy.mSpeed) + mInitPos ;
        Debug.Log("Run!"+mInitPos+ " "+mDestPos);
    
    
    }

    private void OnTriggerEnter(Collider other){
        //if(other.GetComponentInParent)
        if(other.tag == "Enemy")
        {
            other.GetComponent<EnemyBehaviour>().mLife -= 1;
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (mDeltaTime < mTime)
        {
            float lerpFactor = mDeltaTime/mTime; 

            Vector3 pos = Vector3.Lerp(mInitPos, mEnemy.transform.position, lerpFactor);// 
            transform.position = pos;
            transform.position = new Vector3(transform.position.x, mAnimationCurve.Evaluate(lerpFactor)*4, transform.position.z);

            mDeltaTime += Time.deltaTime;   


        }else
        {
            Destroy(gameObject);
        }
        
    }
}
