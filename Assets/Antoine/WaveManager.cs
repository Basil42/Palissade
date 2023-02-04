using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public int round;

    // Ere

    public GameObject ennemiGameObject;

    public void LunchWave()
    {
        for (int i = 0; i < 4; i++)
        {
            Instantiate(ennemiGameObject);
        }
    }
}
