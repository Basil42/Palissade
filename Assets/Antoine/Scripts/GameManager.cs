using System;
using System.Collections;
using UnityEngine;

public enum GameMode
{
    TowerMode,
    RampartMode,
    AttackMode,
    StartingMode
}

public enum Era
{
    Roman,
    Medieval,
    Modern
}

public class GameManager : Singleton<GameManager>
{
    [Min(0)]
    public int round;

    [Tooltip("Nombre de vague d'ennemi pour une époque")]
    public int roundByEra = 3;


    [SerializeField]
    private GameMode gameMode = GameMode.StartingMode; public GameMode ActualGameMode => gameMode;
    [SerializeField]
    private Era actualEra;

    [SerializeField] private InitialPhaseController initialPhaseController;
    public static event Action OnEnterTowerMode;
    public static event Action OnExitTowerMode;
    public static event Action OnEnterWallMode;
    public static event Action OnExitWallMode;
    public static event Action OnEnterAttackMode;
    public static event Action OnExitAttackMode;
    public static event Action OnEnterOpeningMode;
    public static event Action OnExitOpeningMode;

    private IEnumerator Start()
    {
        actualEra = Era.Roman;
        if (gameMode == GameMode.StartingMode)
        {
            OnEnterOpeningMode?.Invoke();
            //Opening phase
            if (initialPhaseController != null)
            {
                yield return StartCoroutine(initialPhaseController.CastleSelection());
            }
            OnExitOpeningMode?.Invoke();
        }
        
        gameMode = GameMode.TowerMode;
        OnEnterTowerMode?.Invoke();
        
    }


    /// <summary>
    /// Charge le mode de jeu suivant
    /// </summary>
    public void NextMode()
    {
        switch (gameMode)
        {
            case GameMode.TowerMode:
                OnExitTowerMode?.Invoke();

                gameMode = GameMode.AttackMode;

                OnEnterAttackMode?.Invoke();
                break;

            case GameMode.RampartMode:
                OnExitWallMode?.Invoke();

                gameMode = GameMode.TowerMode;

                OnEnterTowerMode?.Invoke();
                break;

            case GameMode.AttackMode:
                OnExitAttackMode?.Invoke();
                round++;

                if (round >= roundByEra)
                {
                    round = 0;
                    NewEra();
                    gameMode = GameMode.RampartMode;

                    OnEnterWallMode?.Invoke();
                }
                else
                {
                    gameMode = GameMode.RampartMode;

                    OnEnterWallMode?.Invoke();
                }
                break;

            default:
                Debug.Log("ERREUR, MODE DE JEU INCORRECT");
                break;
        }

            //Debug.Log(gameMode + ", round " + round) ;
    }

    private void NewEra()
    {
        switch (actualEra)
        {
            case Era.Roman:
                actualEra = Era.Medieval;
                break;
            case Era.Medieval:
                actualEra = Era.Modern;
                break;
            case Era.Modern:
                actualEra = Era.Roman;
                break;
            default:
                Debug.Log("ERREUR, ERE NON RECONNUE");
                break;
        }
        Debug.Log($"New era : {actualEra}");

        // METTRE LES STRUCTURES A JOURS
    }
}
