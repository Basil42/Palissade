using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    private float _timer;


    [Tooltip("Dur�e d'une vague d'attaque")]

    public float waveDuration = 10;

    public GameObject ennemiPrefab;

    public bool _waveInProgress;

    [SerializeField, Tooltip("Points d'apparitions des navires")]
    private Transform[] spawShips;

    [SerializeField, Tooltip("Destinations des navires")]
    private Transform[] destinationsShips;

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

    private void OnEnable()
    {
        LunchWave();
    }

    public void LunchWave()
    {
        List<Transform> spawnsFree = new List<Transform>(spawShips);

        int ships = CalculHowManyShips();
        for (int i = 0; i < ships; i++)
        {
            // On prend un spawn aléatoire de la liste
            Transform t = spawnsFree[Random.Range(0, spawnsFree.Count)];
            // On instancie le bateau
            GameObject ship = Instantiate(ennemiPrefab, t.position, t.rotation);
            // On l'enlève de la liste
            spawnsFree.Remove(t);

            // On donne au bateau une destination
            ship.GetComponent<ShipBehavior>().destination = destinationsShips[Random.Range(0, destinationsShips.Length)];
        }

        _waveInProgress = true;
        _timer = 0;
    }

    private int CalculHowManyShips()
    {
        int ships = 3 + GameManager.Instance.round;
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
    }
}
