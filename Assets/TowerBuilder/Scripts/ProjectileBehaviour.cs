using UnityEngine;
public class ProjectileBehaviour : ProjectileBasic
{
//TODO target a position rather than an entity that could be destroyed
    protected override void Fly()
    {
        float lerpFactor = mFlyTimer / mTime;

        Vector3 pos = Vector3.Lerp(mInitPos, mDestPos, lerpFactor);
        
        transform.position = new Vector3(pos.x, mAnimationCurve.Evaluate(lerpFactor) * 4, pos.z);

        mFlyTimer += Time.deltaTime;
    }

    void Update()
    {

        if (mFlyTimer < mTime)
        {
            Fly();
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
