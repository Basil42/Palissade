using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TowerModeController : Singleton<TowerModeController>
{
    [SerializeField] private TowerBuilder towerBuilderRef;
    private void Awake()
    {
        Assert.IsNotNull(towerBuilderRef);
        towerBuilderRef.enabled = false;
        GameManager.OnEnterTowerMode += TowerModeInit;
    }

    private void OnDestroy()
    {
        GameManager.OnEnterTowerMode -= TowerModeInit;
    }

    private void TowerModeInit()
    {
        StartCoroutine(TowerModeInitRoutine());
    }

    IEnumerator TowerModeInitRoutine()
    {
        yield return StartCoroutine(ZoneOfControl.Instance.ZoneOfControlAnimationRoutine());
        towerBuilderRef.enabled = true;
        Debug.Log("tower mode init over");
    }
}
