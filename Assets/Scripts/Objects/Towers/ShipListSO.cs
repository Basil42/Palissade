using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipListSO : MonoBehaviour
{
    private List<ShipBehavior> _shipList;
    public IReadOnlyCollection<ShipBehavior> ShipList => _shipList.AsReadOnly();

    private void OnEnable()
    {
        ShipBehavior.OnShipSpawn += OnNewShip;
        ShipBehavior.OnShipDestroyed += OnShipRemoved;
    }

    private void OnDisable()
    {
        ShipBehavior.OnShipSpawn -= OnNewShip;
        ShipBehavior.OnShipDestroyed -= OnShipRemoved;

    }

    private void OnNewShip(ShipBehavior obj)
    {
        _shipList.Add(obj);
    }
    private void OnShipRemoved(ShipBehavior obj)
    {
        _shipList.Remove(obj);
    }
}
