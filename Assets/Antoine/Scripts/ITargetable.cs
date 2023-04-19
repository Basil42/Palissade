using UnityEngine;

public interface ITargetable
{
    public Vector3 getPosition();
    public float getSpeed();
    public Vector3 getDestination();

    public void getTargetingInfo(out Vector3 position, out float speed, out Vector3 destination);
}