using System.Collections;
using System.Collections.Generic;

/// <summary>
/// alias Cells, Units... bref un emplacement sur la grille
/// </summary>
public class Node
{
    #region Fields

    /// <summary>
    /// Postition du Node dans la grille
    /// </summary>
    private Coords _coords = Coords.Zero; public Coords Coord => _coords;

    /// <summary>
    /// Definis l'état du node
    /// </summary>
    private EnumStateNode _stateNode = EnumStateNode.buildable;

    #endregion

    #region Initialisation

    /// <summary>
    /// Class constructeur
    /// </summary>
    public Node(int x, int y)
    {
        _coords = new Coords(x, y);
    }

    #endregion

    #region Public API

    /// <inheritdoc cref="_coords"/>
    public Coords Position => _coords;

    /// <inheritdoc cref="_nodeState_crossable"/>
    public EnumStateNode StateNode { get { return _stateNode; } set { _stateNode = value; } }

    #endregion
}