using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehavior : MonoBehaviour
{
    public ParticleSystem mVFXDamage;

    public GameObject projectilePrefab;

    public bool isDestroyed = false;

    public Transform destination;

    private GameObject target;

    public float speed = 1f;

    public float life = 5;

    public float speedAttack = 1f;
    private float _timerAttack;

    private void Awake()
    {
        GameManager.OnEnterAttackMode += () =>
        {
            this.enabled = true;
        };
        GameManager.OnExitAttackMode += () =>
        {
            this.enabled = false;
        };
    }

    private void Update()
    {
        // Le bateau avance vers sa destination
        Vector3 dir = destination.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        //Attaque
        _timerAttack += Time.deltaTime;
        if (_timerAttack >= speedAttack)
        {
            ShipAttack();
            _timerAttack = 0;
        }
    }

    private void ShipAttack()
    {
        List<Vector3> targets = new List<Vector3>();
        Level level = FindObjectOfType<LevelManager>().LevelRef;

        for (int y = 0; y < level.Height; y++)
        {
            for (int x = 0; x < level.Width; x++)
            {
                if (level[x,y].StateNode == EnumStateNode.wall)
                {
                    targets.Add(new Vector3(x,0,y));
                }
            }
        }

        if(targets.Count > 0 )
        {
            Vector3 target = targets[Random.Range(0, targets.Count)];
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            projectile.GetComponent<ProjectileBasic>().mDestPos = target;
            Debug.Log("Shoot to " + target.x +" " + target.z);
        }
        else
        {
            Debug.Log("Pas de muraille détectée");
        }
    }

    public void Hit(int damage)
    {
        Debug.Log("SHIP HIT");
        life -= damage;
        mVFXDamage.Play();
        if (life <= 0)
        {
            WaveManager.Instance.shipsList.Remove(this);

            isDestroyed= true;
            GetComponent<Renderer>().enabled = false;
            this.enabled = false;
        }
    }
}
