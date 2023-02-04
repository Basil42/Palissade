using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private int round;

    [Tooltip("Nombre de vague d'ennemi pour une époque")]
    public int roundByEra = 3;

    // Era

    public GameObject ennemiGameObject;

    [SerializeField, Tooltip("Points d'apparitions des navires")]
    private Transform[] spawShips;

    [SerializeField, Tooltip("Destinations des navires")]
    private Transform[] destinationsShips;

    private void Start()
    {
        LunchWave();
    }

    public void LunchWave()
    {
        List<Transform> spawnsFree = new List<Transform>(spawShips);

        for (int i = 0; i < 3 + round; i++)
        {
            // On prend un spawn aléatoire de la liste
            Transform t = spawnsFree[Random.Range(0, spawnsFree.Count)];
            // On instancie le bateau
            GameObject ship = Instantiate(ennemiGameObject, t.position, t.rotation);
            // On l'enlève de la liste
            spawnsFree.Remove(t);

            // On donne au bateau une destination
            ship.GetComponent<ShipBehavior>().destination = destinationsShips[Random.Range(0, destinationsShips.Length)];
        }

        round++;

        if (round >= roundByEra)
        {
            Debug.Log("Dernière vague");
        }
    }
}
