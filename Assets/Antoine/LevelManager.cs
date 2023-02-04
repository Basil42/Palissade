using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Level levelRef;
    
    void Awake()
    {
        levelRef.initializeLevel();
    }
    
}
