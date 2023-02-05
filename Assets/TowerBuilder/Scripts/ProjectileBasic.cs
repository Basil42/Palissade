using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBasic : MonoBehaviour
{
    [Min(1)] public int damage = 2;
    public float mSpeed;
    public float mTime;
    public float mDeltaTime;
    public Vector3 mInitPos;
    public Vector3 mDestPos;
    public AnimationCurve mAnimationCurve;
    protected Collider m_ObjectCollider;
    public ParticleSystem mVFXDamage;



    // Start is called before the first frame update
    void Start()
    {
        m_ObjectCollider = GetComponent<Collider>();
        mInitPos = gameObject.transform.position;
        m_ObjectCollider = GetComponent<Collider>();
        mDeltaTime = 0;
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Test 1 " + other.gameObject.name);
        if(other.TryGetComponent(out ShipBehavior ship))
        {
            Debug.Log("Test 2");
            ship.Hit(damage);
            //mVFXDamage.transform.position = ship.transform.position;
            //mVFXDamage.Play();
        }
    }

    public void Fly()
    {
        float lerpFactor = mDeltaTime / mTime;

        Vector3 pos = Vector3.Lerp(mInitPos, mDestPos, lerpFactor);// 
        transform.position = pos;
        transform.position = new Vector3(transform.position.x, mAnimationCurve.Evaluate(lerpFactor) * 4, transform.position.z);

        mDeltaTime += Time.deltaTime;
    }
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
