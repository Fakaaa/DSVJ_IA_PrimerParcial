using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using PrimerParcial.Gameplay.Map.Handler;

namespace PrimerParcial.Gameplay.Controllers
{
    public class MapController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Vector2Int mapSize = default;
        [SerializeField] private Vector2Int destination = default;
        [SerializeField] private List<Node> map = new List<Node>();
        #endregion

        #region PRIVATE_FIELDS
        private Camera mainCamera = null;
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
            foreach (Node node in map)
            {
                if(!node.IsLocked)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Vector3 worldPosition = new Vector3(node.GetCellPosition().x, node.GetCellPosition().y, 0.0f);
                Gizmos.DrawWireSphere(worldPosition, 0.2f);
                Handles.Label(worldPosition, node.GetCellPosition().ToString(), style);
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init()
        {
            map = new List<Node>();

            for (int i = 0; i < mapSize.y; i++)
            {
                for (int j = 0; j < mapSize.x; j++)
                {
                    int randomChanceOfObstacle = Random.Range(0,100);

                    if(randomChanceOfObstacle < 25)
                    {
                        if(map.Count > 1)
                        {
                            map.Add(new Node(new Vector2Int(j, i), true));
                        }
                        else
                        {
                            map.Add(new Node(new Vector2Int(j, i)));
                        }
                    }
                    else
                    {
                        map.Add(new Node(new Vector2Int(j, i)));
                    }
                }
            }

            pathfinding = new Pathfinding(map);

            Node originNode = map.Find(node => node.GetCellPosition() == new Vector2Int(0, 0));
            Node destinationNode = map.Find(node => node.GetCellPosition() == new Vector2Int(destination.x, destination.y));

            originNode.SetLocked(false);
            destinationNode.SetLocked(false);

            //List<Vector2Int> path = pathfinding.GetPath(originNode.GetCellPosition(), destinationNode.GetCellPosition());
            //
            //for (int i = 0; i < path.Count; i++)
            //{
            //    Debug.Log(path[i]);
            //}

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            mainCamera.transform.position = new Vector3((mapSize.x * 0.5f), (mapSize.y * 0.5f), -10);
        }

        public List<Vector2Int> GetPath(Vector2Int origin, Vector2Int destination)
        {
            Node originNode = map.Find(node => node.GetCellPosition() == new Vector2Int(origin.x, origin.y));
            Node destinationNode = map.Find(node => node.GetCellPosition() == new Vector2Int(destination.x, destination.y));

            return pathfinding.GetPath(originNode.GetCellPosition(), destinationNode.GetCellPosition());
        }

        public Vector2Int GetRandomMapPosition()
        {
            return GetRandomMapNode().GetCellPosition();
        }

        public Node GetMapNodeFromPosition(Vector2Int position)
        {
            return map.Find(node => node.GetCellPosition() == position);
        }

        public Node GetRandomMapNode()
        {
            Vector2Int randomPosition = new Vector2Int(Random.Range(0, mapSize.x), Random.Range(0, mapSize.y));

            return GetMapNodeFromPosition(randomPosition);
        }
        #endregion
    }
}