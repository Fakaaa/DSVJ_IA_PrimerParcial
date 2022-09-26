using UnityEngine;

public class Node
{
    #region PRIVATE_FIELDS
    private Vector2Int cellPosition;
    private int gCost;
    private int hCost;
    private int fCost;
    private Node cameFromNode;
    #endregion

    #region PROPERTIES
    public int GCost { get { return gCost; } }
    public int HCost { get { return hCost; } }
    public int FCost { get { return fCost; } }
    #endregion

    #region CONSTRUCTOR
    public Node(Vector2Int cellPosition)
    {
        this.cellPosition = cellPosition;
    }
    #endregion

    #region PUBLIC_METHODS
    public Vector2Int GetCellPosition()
    {
        return cellPosition;
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
