using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class MapTransitions : MonoBehaviour
{
    
    [SerializeField] private bool startsIn2dMode = false;
    [Header("camera data")]
    public Camera cam;
    [SerializeField] Vector3 neutralCameraPosition3D = new Vector3(121.8f,60f,45f);
    [SerializeField] Vector3 neutralCameraPosition2D = new Vector3(125f, 60f, 81f);
    [SerializeField] Vector3 neutral2DRotation = new Vector3(90, 0, 0);
    [SerializeField] Vector3 neutralCameraRotation = new Vector3(60f,10f,0f);//the position the camera snaps to in 3d mode
    
    [Header("2d map settings")]
    [SerializeField] SpriteRenderer spriteMapRenderer;//keeping a direct reference to the material might be better
    [SerializeField] private float transitionDuration = 2.5f;
    [SerializeField] private float cameraSnappingDuration = 0.5f;
    
    private readonly int _transitionValueId = Shader.PropertyToID("_transitionValue");
    private LocalKeyword _transitionInOutKeyWord;
    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.OnEnterWallMode += ChangeGraphicsTo2D;
        GameManager.OnExitWallMode += ChangeGraphicsTo3D;
        foreach (var keyword in spriteMapRenderer.sharedMaterial.shader.keywordSpace.keywords)
        {
            if (keyword.name == "_TRANSITIONINOUT_ON")
            {
                _transitionInOutKeyWord = keyword;
                break;
            }
        }
        spriteMapRenderer.sharedMaterial.SetKeyword(_transitionInOutKeyWord,startsIn2dMode);
        spriteMapRenderer.sharedMaterial.SetFloat(_transitionValueId,1.0f);
    }

    private void OnDestroy()
    {
        GameManager.OnEnterWallMode -= ChangeGraphicsTo2D;
        GameManager.OnExitWallMode -= ChangeGraphicsTo3D;
    }

    void Start()
    {
        var camTransform = cam.transform;
        camTransform.position = startsIn2dMode ? neutralCameraPosition2D: neutralCameraPosition3D;
        camTransform.eulerAngles = startsIn2dMode ? neutral2DRotation : neutralCameraRotation;
    }

    private void ChangeGraphicsTo3D()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeGraphicsTo3DRoutine());
    }
    private IEnumerator ChangeGraphicsTo3DRoutine()
    {
        var camTransform = cam.transform;
        var mat = spriteMapRenderer.sharedMaterial;
        var timer = 0f;
        mat.SetKeyword(_transitionInOutKeyWord,false);
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            mat.SetFloat(_transitionValueId, timer/transitionDuration);
            yield return null;
        }
        timer = 0f;

        while (timer < cameraSnappingDuration)
        {
            timer += Time.deltaTime;
            var eulerRotation = Vector3.Slerp(neutral2DRotation, neutralCameraRotation, timer / cameraSnappingDuration);
            var position = Vector3.Lerp(neutralCameraPosition2D, neutralCameraPosition3D, timer / cameraSnappingDuration);
            camTransform.position = position;
            camTransform.eulerAngles = eulerRotation;
            yield return null;
        }
    }

    private void ChangeGraphicsTo2D()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeGraphicsTo2DRoutine());
    }
    private IEnumerator ChangeGraphicsTo2DRoutine()
    {
        var camTransform = cam.transform;
        var mat = spriteMapRenderer.sharedMaterial;
        var timer = 0f;while (timer < cameraSnappingDuration)
        {
            timer += Time.deltaTime;
            var eulerRotation = Vector3.Slerp(neutralCameraRotation,  neutral2DRotation, timer / cameraSnappingDuration);
            var position = Vector3.Lerp(neutralCameraPosition3D, neutralCameraPosition2D, timer / cameraSnappingDuration);
            camTransform.position = position;
            camTransform.eulerAngles = eulerRotation;
            yield return null;
        }
        mat.SetKeyword(_transitionInOutKeyWord,true);
        timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            mat.SetFloat(_transitionValueId, timer/transitionDuration);
            yield return null;
        }

        
        
    }
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    [SerializeField] private bool DebugDisplay = false;
    private void OnGUI()
    {
        if (DebugDisplay)
        {
            if (GUILayout.Button("2D view",GUILayout.Height(64), GUILayout.Width(128)))
            {
                ChangeGraphicsTo2D();
            }

            if (GUILayout.Button("3d view",GUILayout.Height(64), GUILayout.Width(128)))
            {
                ChangeGraphicsTo3D();
            }
        }
    }
#endif
}
