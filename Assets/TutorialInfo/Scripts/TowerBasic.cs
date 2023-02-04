using System.Collections;
using System.Collections.Generic;
// using UnityEngine.CoreModule;
using UnityEngine;

public class TowerBasic : MonoBehaviour
{
    public List<EnemyBehaviour> mEnemies = new List<EnemyBehaviour>();
    // public List<GameObject> mEnemies;
    public int mLife;

    public ProjectileBehaviour mProjectile; 

    void Shoot(){
        // cherche un enemis et lui tire dessus


        // 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Damage(EnemyBehaviour enemy){
        enemy.mLife -= 1;
    }
    public void SortEnemy(){
        mEnemies.Sort((enemy, enemy2) =>
        {
            if (enemy == null && enemy2 == null) return 0;
            else if (enemy == null) return -1;
            else if (enemy2 == null) return 1;
            else return (Vector3.Distance(transform.position, enemy.gameObject.transform.position) < Vector3.Distance(transform.position, enemy2.gameObject.transform.position)) ? -1 : 1;
        });
        // Vector3.Distance(transform.position, obj.transform.position)
    }

    public void ManageDeadEnemy(){
        for (int i = 0; i < mEnemies.Count; i++)
        {
            if(mEnemies[i].isDead()){
                Debug.Log(mEnemies[i].gameObject.name + " is dead");
                Destroy(mEnemies[i].gameObject);
                mEnemies.Remove(mEnemies[i]);

            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {


    }
}
