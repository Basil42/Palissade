using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ConstructibleBehavior : MonoBehaviour
{
    private RaycastHit _hit;
    private Transform _transRef;

    void Update()
    {
#if UNITY_EDITOR
        _transRef = transform;
        if (transform.hasChanged && Physics.Raycast(new Vector3(_transRef.position.x,40f,_transRef.position.z),Vector3.down,out _hit))
        {
            var position = _transRef.position;
            _transRef.position = new Vector3(position.x, _hit.point.y, position.z);
        }
#endif
    }
}