using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehavior : MonoBehaviour
{
    public Transform destination;

    private GameObject target;

    public float speed = 1f;

    public float life = 5;

    public float speedAttack = 1f;
    private float _timerAttack;

    private void Update()
    {
        // Le bateau avance vers sa destination
        Vector3 dir = destination.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        //Attaque
        _timerAttack += Time.deltaTime;
        if (_timerAttack >= speedAttack)
        {
            //ATTACK
            _timerAttack = 0;
        }
    }
}
