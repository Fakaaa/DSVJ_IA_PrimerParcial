using System.Collections.Generic;

using UnityEngine;

using PrimerParcial.Gameplay.Map.Data;

namespace PrimerParcial.Gameplay.Map.Handler
{
    public class Pathfinding
    {
        #region PUBLIC_METHODS
        public List<Vector2Int> GetPath(MapNode[] map, MapNode origin, MapNode destination)
        {
            MapNode currentNode = origin;
            while (currentNode.position != destination.position)
            {
                currentNode.state = MapNode.NodeState.Closed;

                for (int i = 0; i < currentNode.adjacentNodeIDs.Count; i++)
                {
                    if (currentNode.adjacentNodeIDs[i] != -1)
                    {
                        if (map[currentNode.adjacentNodeIDs[i]].state == MapNode.NodeState.Ready)
                        {
                            map[currentNode.adjacentNodeIDs[i]].Open(currentNode.ID);
                        }
                    }
                }

                currentNode = GetNextNode(map, currentNode);
                if (currentNode == null)
                    return new List<Vector2Int>();
            }

            List<Vector2Int> path = GeneratePath(map, currentNode);
            foreach (MapNode node in map)
            {
                node.Reset();
            }
            return path;
        }
        #endregion

        #region PRIVATE_METHODS
        private List<Vector2Int> GeneratePath(MapNode[] map, MapNode current)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            while (current.openerID != -1)
            {
                path.Add(current.position);
                current = map[current.openerID];
            }

            path.Reverse();

            return path;
        }

        private MapNode GetNextNode(MapNode[] map, MapNode currentNode)
        {
            for (int i = 0; i < currentNode.adjacentNodeIDs.Count; i++)
            {
                if (currentNode.adjacentNodeIDs[i] != -1)
                {
                    if (map[currentNode.adjacentNodeIDs[i]].state == MapNode.NodeState.Open)
                    {
                        if (map[currentNode.adjacentNodeIDs[i]].openerID == currentNode.ID)
                        {
                            return map[currentNode.adjacentNodeIDs[i]];
                        }
                    }
                }
            }

            if (currentNode.openerID == -1)
                return null;

            return GetNextNode(map, map[currentNode.openerID]);
        }
        #endregion
    }
}