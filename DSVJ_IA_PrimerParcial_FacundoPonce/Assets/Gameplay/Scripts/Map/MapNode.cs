using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrimerParcial.Gameplay.Map.Data
{
    [Serializable]
    public class MapNode
    {
        #region ENUM
        public enum NodeState
        {
            Open,//Abiertos por otro nodo pero no visitados
            Closed,//ya visitados
            Ready//no abiertos por nadie
        }
        #endregion

        #region EXPOSED_FIELDS
        public int ID;
        public Vector2Int position;
        public List<int> adjacentNodeIDs;
        public NodeState state;
        public int openerID;
        #endregion

        #region CONSTRUCTOR
        public MapNode(int ID, Vector2Int position)
        {
            this.ID = ID;
            this.position = position;
            this.adjacentNodeIDs = NodeUtils.GetAdjacentsNodeIDs(position);
            this.state = NodeState.Ready;
            this.openerID = -1;
        }
        #endregion

        #region PUBLIC_METHODS
        public void Open(int openerID)
        {
            state = NodeState.Open;
            this.openerID = openerID;
        }

        public void Reset()
        {
            this.state = NodeState.Ready;
            this.openerID = -1;
        }
        #endregion
    }

    public static class NodeUtils
    {
        public static Vector2Int MapSize;

        public static List<int> GetAdjacentsNodeIDs(Vector2Int position)
        {
            List<int> IDs = new List<int>();
            IDs.Add(PositionToIndex(new Vector2Int(position.x + 1, position.y)));
            IDs.Add(PositionToIndex(new Vector2Int(position.x, position.y - 1)));
            IDs.Add(PositionToIndex(new Vector2Int(position.x - 1, position.y)));
            IDs.Add(PositionToIndex(new Vector2Int(position.x, position.y + 1)));
            return IDs;
        }

        public static int PositionToIndex(Vector2Int position)
        {
            if (position.x < 0 || position.x >= MapSize.x ||
                position.y < 0 || position.y >= MapSize.y)
                return -1;
            return position.y * MapSize.x + position.x;
        }
    }
}