using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : Singleton<WaveManager>
{
    private float _timer;


    [Tooltip("Dur�e d'une vague d'attaque")]

    public float waveDuration = 10;

    public GameObject ennemiPrefab;

    public bool _waveInProgress;

    [SerializeField, Tooltip("Points d'apparitions des navires")]
    private Vector2Int[] spawShips;

    [SerializeField, Tooltip("Destinations des navires")]
    private Vector2Int[] destinationsShips;

    public List<ShipBehavior> shipsList = new List<ShipBehavior>();

    
    private void Awake()
    {
        GameManager.OnEnterAttackMode += () =>
        {
            this.enabled = true;
            Debug.Log("Attack mode");
        };
        GameManager.OnExitAttackMode += () =>
        {
            this.enabled = false;
            Debug.Log("Attack mode stop");
        };
    }

    private Level LevelRef;
    private void OnEnable()
    {
        LevelRef = LevelManager.Instance.LevelRef;
        LaunchWave();
        
    }

    public void LaunchWave()
    {
        try
        {
            List<Vector2Int> spawnsFree = new List<Vector2Int>(spawShips);//TODO: replace transform reference by an IntVector

            int ships = CalculHowManyShips();
            for (int i = 0; i < ships; i++)
            {
                // On prend un spawn aléatoire de la liste
                Vector2Int SpawnCoord = spawnsFree[Random.Range(0, spawnsFree.Count)];//TODO: fallback behavior if this list is empty
                // On instancie le bateau
                GameObject ship = Instantiate(ennemiPrefab, LevelRef.GetCenterWorldPosition(SpawnCoord),Quaternion.identity);//TODO: leave the ship orient itself on instantiation
                // On l'enlève de la liste
                spawnsFree.Remove(SpawnCoord);

                // On donne au bateau une destination
                ship.GetComponent<ShipBehavior>().destination = LevelRef.GetCenterWorldPosition(destinationsShips[Random.Range(0, destinationsShips.Length)]);

                // On ajoute le bateau � la liste des bateaux acitfs
                shipsList.Add(ship.GetComponent<ShipBehavior>());
            }

            _waveInProgress = true;
            _timer = 0;
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private int CalculHowManyShips()
    {
        int ships = 3 + GameManager.Instance.round;//TODO: fix this
        return ships;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if(_waveInProgress && _timer >= waveDuration)
        {
            Debug.Log("Fin de la vague");
            _waveInProgress = false;
            GameManager.Instance.NextMode();
        }
        else if(shipsList.Count <= 0)
        {
            Debug.Log("Fin de la vague par manque d'ennemi");
            _waveInProgress = false;
            GameManager.Instance.NextMode();
        }
    }
}
