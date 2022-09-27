using System.Collections.Generic;

using UnityEngine;

namespace PrimerParcial.Gameplay.Map.Handler
{
    [System.Serializable]
    public class Pathfinding
    {
        #region PRIVATE_FIELDS
        private List<Node> map = new List<Node>();
        #endregion

        #region CONSTRUCTOR
        public Pathfinding(List<Node> map)
        {
            this.map = new List<Node>();

            for (int i = 0; i < map.Count; i++)
            {
                if (map[i] != null)
                {
                    if(!map[i].IsLocked)
                    {
                        this.map.Add(map[i]);
                    }
                }
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public List<Vector2Int> GetPath(Vector2Int origin, Vector2Int destination)
        {
            bool originLocated = false;
            bool endLocated = false;

            Node originNode = null;
            Node destinationNode = null;

            for (int i = 0; i < map.Count; i++)
            {
                if (map[i] != null)
                {
                    if (originLocated && endLocated)
                        break;

                    if (map[i].GetCellPosition() == origin)
                    {
                        originNode = map[i];
                        originLocated = true;
                    }
                    if(map[i].GetCellPosition() == destination)
                    {
                        destinationNode = map[i];
                        endLocated = true;
                    }
                }
            }

            if (!originLocated || !endLocated)
            {
                Debug.Log("ORIGIN OR END NOT LOCATED");
                return null;
            }

            List<Node> openList = new List<Node>() { originNode };
            List<Node> closedList = new List<Node>();

            for (int i = 0; i < map.Count; i++)
            {
                if (map[i] != null)
                {
                    map[i].SetGCost(int.MaxValue);
                    map[i].CalculateFCost();
                    map[i].SetCameFromCell(null);
                }
            }

            originNode.SetGCost(0);
            originNode.SetHCost(CalculateDistanceCost(originNode, destinationNode));
            originNode.CalculateFCost();

            while(openList.Count > 0)
            {
                Node actualNode = GetLowestFCostNode(openList);

                if (actualNode == destinationNode)
                {
                    return CalculatePath(destinationNode);
                }

                openList.Remove(actualNode);
                closedList.Add(actualNode);

                foreach (Node neighbourCell in GetNeighboursList(actualNode, map))
                {
                    if(closedList.Contains(neighbourCell))
                    {
                        continue;
                    }

                    int tentativeCost = actualNode.GCost + CalculateDistanceCost(actualNode, neighbourCell);
                    if(tentativeCost < neighbourCell.GCost)
                    {
                        neighbourCell.SetCameFromCell(actualNode);
                        neighbourCell.SetGCost(tentativeCost);
                        neighbourCell.SetHCost(CalculateDistanceCost(neighbourCell, destinationNode));
                        neighbourCell.CalculateFCost();
                    }
                    if(!openList.Contains(neighbourCell))
                    {
                        openList.Add(neighbourCell);
                    }
                }
            }

            //No encontro camino.
            return null;
        }        
        #endregion

        #region PRIVATE_METHODS
        private int CalculateDistanceCost(Node actualNode, Node destination)
        {
            int diagonalCost = 14;
            int straightCost = 10;
            
            int xDistance = Mathf.Abs(actualNode.GetCellPosition().x - destination.GetCellPosition().x);
            int yDistance = Mathf.Abs(actualNode.GetCellPosition().y - destination.GetCellPosition().y);
            int remaining = Mathf.Abs(xDistance - yDistance);

            return diagonalCost * Mathf.Min(xDistance, yDistance) + straightCost * remaining;
        }

        private Node GetLowestFCostNode(List<Node> openList)
        {
            Node nodeWithLowestCost = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i] != null)
                {
                    if(openList[i].FCost < nodeWithLowestCost.FCost)
                    {
                        nodeWithLowestCost = openList[i];
                    }
                }
            }

            return nodeWithLowestCost;
        }
        
        private List<Node> GetNeighboursList(Node actualNode, List<Node> map)
        {
            List<Node> neighboursList = new List<Node>();

            int x = actualNode.GetCellPosition().x;
            int y = actualNode.GetCellPosition().y;

            for (int i = 0; i < map.Count; i++)
            {
                if (map[i] != null)
                {
                    if( map[i].GetCellPosition() == new Vector2Int(x -1, y)||
                        map[i].GetCellPosition() == new Vector2Int(x - 1, y - 1)||
                        map[i].GetCellPosition() == new Vector2Int(x - 1, y + 1)||
                        map[i].GetCellPosition() == new Vector2Int(x + 1, y)||
                        map[i].GetCellPosition() == new Vector2Int(x + 1, y - 1)||
                        map[i].GetCellPosition() == new Vector2Int(x + 1, y + 1)||
                        map[i].GetCellPosition() == new Vector2Int(x, y - 1)||
                        map[i].GetCellPosition() == new Vector2Int(x, y + 1))
                    {
                        neighboursList.Add(map[i]);
                    }
                }
            }

            return neighboursList;
        }

        private List<Vector2Int> CalculatePath(Node destinationNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            path.Add(destinationNode.GetCellPosition());
            Node currentNode = destinationNode;

            while (currentNode.GetCameFromNode() != null)
            {
                path.Add(currentNode.GetCameFromNode().GetCellPosition());
                currentNode = currentNode.GetCameFromNode();
            }
            path.Reverse();
            return path;
        }
        #endregion
    }
}