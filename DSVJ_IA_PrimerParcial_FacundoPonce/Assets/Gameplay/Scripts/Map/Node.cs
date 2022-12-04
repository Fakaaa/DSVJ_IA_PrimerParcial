using UnityEngine;

[System.Serializable]
public class Node
{
    #region PRIVATE_FIELDS
    private Vector2 cellPosition;
    private float gCost;
    private float hCost;
    private float fCost;
    private Node cameFromNode;
    private Vector2Int gridPosition = default;

    private bool isLocked = false; //This is like an obstacle.
    #endregion

    #region PROPERTIES
    public bool IsLocked { get { return isLocked; } }
    public float GCost { get { return gCost; } }
    public float HCost { get { return hCost; } }
    public float FCost { get { return fCost; } }
    public Vector2Int GridPosition => gridPosition;
    #endregion

    #region CONSTRUCTOR
    public Node(Vector2 cellPosition, Vector2Int gridPosition ,bool isLocked = false)
    {
        this.gridPosition = gridPosition;
        this.cellPosition = cellPosition;
        this.isLocked = isLocked;
    }
    #endregion

    #region PUBLIC_METHODS
    public Vector2 GetCellPosition()
    {
        return cellPosition;
    }

    public void SetLocked(bool isLocked)
    {
        this.isLocked = isLocked;
    }

    public void SetGCost(float value)
    {
        gCost = value;
    }

    public void SetHCost(float value)
    {
        hCost = value;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetCameFromCell(Node previousCell)
    {
        cameFromNode = previousCell;
    }

    public Node GetCameFromNode()
    {
        return cameFromNode;
    }
    #endregion
}
