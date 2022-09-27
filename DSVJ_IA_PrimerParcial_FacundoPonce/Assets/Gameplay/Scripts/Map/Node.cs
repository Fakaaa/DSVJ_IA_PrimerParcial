using UnityEngine;

public class Node
{
    #region PRIVATE_FIELDS
    private Vector2Int cellPosition;
    private int gCost;
    private int hCost;
    private int fCost;
    private Node cameFromNode;

    private bool isLocked = false; //This is like an obstacle.
    #endregion

    #region PROPERTIES
    public bool IsLocked { get { return isLocked; } }
    public int GCost { get { return gCost; } }
    public int HCost { get { return hCost; } }
    public int FCost { get { return fCost; } }
    #endregion

    #region CONSTRUCTOR
    public Node(Vector2Int cellPosition, bool isLocked = false)
    {
        this.cellPosition = cellPosition;
        this.isLocked = isLocked;
    }
    #endregion

    #region PUBLIC_METHODS
    public Vector2Int GetCellPosition()
    {
        return cellPosition;
    }

    public void SetLocked(bool isLocked)
    {
        this.isLocked = isLocked;
    }

    public void SetGCost(int value)
    {
        gCost = value;
    }

    public void SetHCost(int value)
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
