using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectileBasic : MonoBehaviour
{
    #region Obsolete

    // [Min(1)] public int damage = 2;
    // public float mSpeed;
    // public float mTime;
    // [FormerlySerializedAs("mDeltaTime")] public float mFlyTimer;
    // public Vector3 mInitPos;
    // public Vector3 mDestPos;
    // public AnimationCurve mAnimationCurve;
    // protected Collider m_ObjectCollider;
    //
    //
    // // Start is called before the first frame update
    // protected virtual void Start()
    // {
    //     m_ObjectCollider = GetComponent<Collider>();
    //     mInitPos = gameObject.transform.position;
    //     m_ObjectCollider = GetComponent<Collider>();
    //     mFlyTimer = 0;
    //     
    // }
    // private void OnTriggerEnter(Collider other)
    // {
    //     if(other.TryGetComponent(out ShipBehavior ship))
    //     {
    //         ship.Hit(damage);
    //     }
    // }
    //
    // protected virtual void Fly()
    // {
    //     float lerpFactor = mFlyTimer / mTime;
    //
    //     Vector3 pos = Vector3.Lerp(mInitPos, mDestPos, lerpFactor);// 
    //     transform.position = pos;
    //     transform.position = new Vector3(transform.position.x, mAnimationCurve.Evaluate(lerpFactor) * 4, transform.position.z);
    //
    //     mFlyTimer += Time.deltaTime;
    // }
    // void Update()
    // {
    //     if (mFlyTimer < mTime)
    //     {
    //         Fly();
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    #endregion
}
