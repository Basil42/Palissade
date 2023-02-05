using System.Collections;
using System.Collections.Generic;
// using UnityEngine.CoreModule;
using UnityEngine;

public class TowerBasic : MonoBehaviour
{
    //public List<ShipBehavior> mEnemies = new List<ShipBehavior>();
    // public List<GameObject> mEnemies;
    public int mLife;
    public ParticleSystem m_vfxShoot;

    public ProjectileBasic mProjectile;
    public void playFXShoot()
    {
        m_vfxShoot.Play();
    }
    void Shoot()
    {
    }
}
