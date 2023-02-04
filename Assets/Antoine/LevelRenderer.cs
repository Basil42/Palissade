using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[AddComponentMenu("Game/Level")]
public class LevelRenderer : MonoBehaviour
{
    public Camera currentCamera;

    [SerializeField, Min(1)]
    private int _widht =  4;

    [SerializeField, Min(1)]
    private int _height = 4;

    [SerializeField]
    private GameObject _tilePrefab = null;

    private Level _level = null;
    /// <summary>
    /// Create the level if needed, and return it
    /// </summary>
    public Level Level
    {
        get
        {
            if(_level == null)
            {
                _level = new Level(_widht, _height);
            }
            return _level;
        }
    }

    public Node actualNodeSelected;

    private void Update()
    {
        //Capte la position de la souris
        Vector3 mousePosition = Input.mousePosition;

        // Rayon de la souris par rapport à la caméra
        Ray ray = currentCamera.ScreenPointToRay(mousePosition);
        // Création d'une variable Raycast nécessaire pour le out de la fonction Raycast
        RaycastHit hit;

        //Si le raycast de la souris sur l'écran est en contact avec un objet de layer "Structure"
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Tile")))
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);

            actualNodeSelected = GetNodeWithCoords(hit.collider.transform.position.x, hit.transform.position.z);
        }
        else
        {
            actualNodeSelected = null;
        }
    }

    public Node GetNodeWithCoords(float x, float z)
    {
        return _level[(int)x, (int)z];
    }

    public void GenerateLevel()
    {
        DestroyAllChilds();

        _level = new Level(_widht, _height);

        // On instancie des tiles sur chaques positions de nodes du level
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _widht; x++)
            {
                GameObject tile = Instantiate(_tilePrefab.gameObject, transform.position + new Vector3(x, 0 ,y), Quaternion.identity, transform);
                tile.name = $"Case [{x}.{y}]";

                // On colorie la tile selon l'état du node
                // NE FONCTIONNE PAS AVEC LE MODE EDITION : SHARED MATERIAL ECRASE LA COULEUR DU MATERIAL DONC TOUT EST
                // DE LA COULEUR DE LA DERNIERE TILE
                switch (_level[x, y].StateNode)
                {
                    case EnumStateNode.water:
                        tile.GetComponent<MeshRenderer>().sharedMaterial.color = Color.blue;
                        break;
                    case EnumStateNode.buildable:
                        tile.GetComponent<MeshRenderer>().sharedMaterial.color = Color.green;
                        break;
                    case EnumStateNode.building:
                        tile.GetComponent<MeshRenderer>().sharedMaterial.color = Color.grey;
                        break;
                    default:
                        tile.GetComponent<MeshRenderer>().sharedMaterial.color = Color.white;
                        break;
                }
            }
        }
    }

    public void DestroyAllChilds()
    {
        Transform[] childs =  gameObject.GetComponentsInChildren<Transform>();

        foreach(Transform child in childs)
        {
            if(child.gameObject != gameObject)
            DestroyImmediate(child.gameObject);
        }
    }

    /*// Gizmos qui affiche la map
    
    private void OnDrawGizmos()
    {
        // Si on est en EditModes && la taille à changé, on recrée le level
        if(!Application.isPlaying && (_widht != Level.Width || _height != Level.Height))
        {
            _level = null;
        }

        for (int y = 0; y < Level.Height; y++)
        {
            for (int x = 0; x < Level.Width; x++)
            {
                switch(Level[x,y].StateNode)
                {
                    case EnumStateNode.water:
                        Gizmos.color = Color.blue;
                        break;
                    case EnumStateNode.buildable:
                        Gizmos.color = Color.green;
                        break;
                    case EnumStateNode.building:
                        Gizmos.color = Color.grey;
                        break;
                    default:
                        Gizmos.color = Color.white;
                        break;
                }

                Gizmos.DrawCube(new Vector3(x,y) + transform.position, Vector2.one); // _height-
            }
        }
    }*/
}