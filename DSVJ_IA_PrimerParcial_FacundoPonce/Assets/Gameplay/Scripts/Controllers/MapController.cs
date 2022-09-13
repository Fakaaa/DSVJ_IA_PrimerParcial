using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using PrimerParcial.Gameplay.Map.Data;
using PrimerParcial.Gameplay.Map.Handler;

namespace PrimerParcial.Gameplay.Controllers
{
    public class MapController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Vector2Int mapSize = default;
        [SerializeField] private Vector2Int destination = default;
        #endregion

        #region PRIVATE_FIELDS
        private Camera mainCamera = null;
        private MapNode[] map = null;
        private Pathfinding pathfinding = null;
        #endregion

        #region PROPERTIES
        public Vector2Int MapSize { get => mapSize; }
        #endregion

        #region UNITY_CALLS
        private void OnDrawGizmos()
        {
            if (map == null)
                return;
            Gizmos.color = Color.green;
            GUIStyle style = new GUIStyle() { fontSize = 10 };
            foreach (MapNode node in map)
            {
                Vector3 worldPosition = new Vector3(node.position.x, node.position.y, 0.0f);
                Gizmos.DrawWireSphere(worldPosition, 0.2f);
                Handles.Label(worldPosition, node.position.ToString(), style);
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init()
        {
            pathfinding = new Pathfinding();
            NodeUtils.MapSize = mapSize;
            map = new MapNode[mapSize.x * mapSize.y];
            int ID = 0;
            for (int i = 0; i < mapSize.y; i++)
            {
                for (int j = 0; j < mapSize.x; j++)
                {
                    map[ID] = new MapNode(ID, new Vector2Int(j, i));
                    ID++;
                }
            }

            List<Vector2Int> path = pathfinding.GetPath(map,
                map[NodeUtils.PositionToIndex(new Vector2Int(0, 0))],
                map[NodeUtils.PositionToIndex(new Vector2Int(destination.x, destination.y))]);

            for (int i = 0; i < path.Count; i++)
            {
                Debug.Log(path[i]);
            }

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            mainCamera.transform.position = new Vector3((mapSize.x * 0.5f), (mapSize.y * 0.5f), -10);
        }

        public Vector2Int GetRandomMapPosition()
        {
            return GetRandomMapNode().position;
        }

        public MapNode GetRandomMapNode()
        {
            return map[NodeUtils.PositionToIndex(new Vector2Int(Random.Range(0, mapSize.x), Random.Range(0, mapSize.y)))];
        }
        #endregion
    }
}