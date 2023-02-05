using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// alias Cells, Units... bref un emplacement sur la grille
/// </summary>
public class Node
{
    #region Fields

    /// <summary>
    /// Postition du Node dans la grille
    /// </summary>
    private Vector2Int _coords = Vector2Int.zero; public Vector2Int Coord => _coords;

    /// <summary>
    /// Definis l'état du node
    /// </summary>
    private EnumStateNode _stateNode = EnumStateNode.buildable;

    private EnumNodeControl _nodeControl = EnumNodeControl.none;
    #endregion

    #region Initialisation

    /// <summary>
    /// Class constructeur
    /// </summary>
    public Node(int x, int y)
    {
        _coords = new Vector2Int(x, y);
    }

    #endregion

    #region Public API

    /// <inheritdoc cref="_coords"/>
    public Vector2Int Position => _coords;

    /// <inheritdoc cref="_nodeState_crossable"/>
    public EnumStateNode StateNode { get { return _stateNode; } set { _stateNode = value; } }
    public EnumNodeControl NodeController
    {
        get { return _nodeControl; }
        set { _nodeControl = value; }
    }
    #endregion
}