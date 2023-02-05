using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = System.Numerics.Quaternion;

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
    private GameMode gameMode = GameMode.TowerMode; public GameMode ActualGameMode => gameMode;
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
        //TODO: snap the camera to 2D with an instant version of the transition
        gameMode = GameMode.TowerMode;
        OnEnterTowerMode?.Invoke();
        cam.transform.position = NeutralCameraPosition2D;
        cam.transform.eulerAngles = Neutral2DRotation;
        cam.cullingMask = mode2D;
        isMode2D = true;
    }

    [SerializeField] Vector3 NeutralCameraPosition3D = new Vector3(121.8f,60f,45f);
    [SerializeField] Vector3 NeutralCameraPosition2D = new Vector3(125f, 60f, 81f);
    [SerializeField] Vector3 Neutral2DRotation = new Vector3(90, 0, 0);
    [SerializeField] Vector3 NeutralCameraRotation = new Vector3(60f,10f,0f);//the position the camera snaps to in 3d mode
    /// <summary>
    /// FlipFlop le mode graphique
    /// </summary>
    public void ChangeGraphicMode()
    {
        StopAllCoroutines();
        if (isMode2D)
        {
            StartCoroutine(ChangeGraphicsTo3DRoutine());
            isMode2D = false;
        }
        else
        {
            cam.cullingMask = mode2D;
            isMode2D = true;
        }
    }
    [Header("Transition")]
    [SerializeField] SpriteRenderer SpriteMapRenderer;
    
    [SerializeField] private float transitionDuration = 2.5f;
    [FormerlySerializedAs("cameraSappingDuration")] [SerializeField] private float cameraSnappingDuration = 0.5f;
    private IEnumerator ChangeGraphicsTo3DRoutine()
    {
        var mat = SpriteMapRenderer.material;
        var timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            mat.SetFloat("transitionValue", 1f - timer/transitionDuration);
            yield return null;
        }
        cam.cullingMask = mode3D;
        timer = 0f;

        while (timer < cameraSnappingDuration)
        {
            timer += Time.deltaTime;
            var eulerRotation = Vector3.Slerp(Neutral2DRotation, NeutralCameraRotation, timer / cameraSnappingDuration);
            var position = Vector3.Lerp(NeutralCameraPosition2D, NeutralCameraPosition3D, timer / cameraSnappingDuration);
            cam.transform.position = position;
            cam.transform.eulerAngles = eulerRotation;
            yield return null;
        }
    }
    private IEnumerator ChangeGraphicsTo2DRoutine()
    {
        var mat = SpriteMapRenderer.material;
        var timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            mat.SetFloat("transitionValue", timer/transitionDuration);
            yield return null;
        }

        timer = 0f;
        while (timer < cameraSnappingDuration)
        {
            timer += Time.deltaTime;
            var eulerRotation = Vector3.Slerp(NeutralCameraRotation,  Neutral2DRotation, timer / cameraSnappingDuration);
            var position = Vector3.Lerp(NeutralCameraPosition3D, NeutralCameraPosition2D, timer / cameraSnappingDuration);
            cam.transform.position = position;
            cam.transform.eulerAngles = eulerRotation;
            yield return null;
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
                    ChangeGraphicMode();
                    gameMode = GameMode.RampartMode;

                    OnEnterWallMode?.Invoke();
                }
                else
                {
                    ChangeGraphicMode();
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
