using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    TowerMode,
    RampartMode,
    AttackMode
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

    public Camera cam;

    [SerializeField]
    private GameMode gameMode = GameMode.TowerMode;
    [SerializeField]
    private Era actualEra;

    public LayerMask mode2D;
    public LayerMask mode3D;

    private bool isMode2D;

    public static Action OnEnterTowerMode;
    public static Action OnExitTowerMode;
    public static Action OnEnterWallMode;
    public static Action OnExitWallMode;
    public static Action OnEnterAttackMode;
    public static Action OnExitAttackMode;
    private void Start()
    {
        actualEra = Era.Roman;

        gameMode = GameMode.TowerMode;
        OnEnterAttackMode?.Invoke();
        cam.cullingMask = mode2D;
        isMode2D = true;
    }

    /// <summary>
    /// FlipFlop le mode graphique
    /// </summary>
    public void ChangeGraphicMode()
    {
        if (isMode2D)
        {
            cam.cullingMask = mode3D;
            isMode2D = false;
        }
        else
        {
            cam.cullingMask = mode2D;
            isMode2D = true;
        }
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
                // On passe en 3D
                ChangeGraphicMode();
                gameMode = GameMode.AttackMode;
                
                WaveManager.Instance.LunchWave();
                OnEnterAttackMode?.Invoke();
                break;

            case GameMode.RampartMode:
                OnExitWallMode?.Invoke();
                gameMode = GameMode.TowerMode;
                
                // Appel placementSystem
                OnEnterTowerMode?.Invoke();
                break;

            case GameMode.AttackMode:
                OnExitAttackMode?.Invoke();
                round++;

                if (round >= roundByEra)
                {
                    round = 0;
                    NewEra();
                    ChangeGraphicMode();
                    gameMode = GameMode.RampartMode;
                    // Appel placementSystem
                    OnEnterWallMode?.Invoke();
                }
                else
                {
                    ChangeGraphicMode();
                    gameMode = GameMode.RampartMode;
                    // Appel placementSystem
                    OnEnterWallMode?.Invoke();
                }
                break;

            default:
                Debug.Log("ERREUR, MODE DE JEU INCORRECT");
                break;
        }

        Debug.Log(gameMode + ", round " + round) ;
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
