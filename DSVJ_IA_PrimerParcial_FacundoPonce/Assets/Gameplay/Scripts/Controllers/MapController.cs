using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using PrimerParcial.Gameplay.Map.Handler;

namespace PrimerParcial.Gameplay.Controllers
{
    public class MapController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Sprite walkeableTile = null;
        [SerializeField] private Sprite blockedTile = null;
        [SerializeField] private SpriteRenderer prefabTile = null;
        [SerializeField] private Vector2Int mapSize = default;
        [SerializeField] private List<Node> allMap = new List<Node>();
        [SerializeField] private Pathfinding pathfinding = null;
        #endregion

        #region PRIVATE_FIELDS
        private Camera mainCamera = null;

        private bool mapInitialized = false;
        #endregion

        #region PROPERTIES
        public List<Node> WalkeableMap { get { return pathfinding.Map; } }  
        public Vector2Int MapSize { get => mapSize; }
        #endregion

        #region UNITY_CALLS
        private void OnDrawGizmos()
        {
            if (allMap == null)
                return;
            Gizmos.color = Color.green;
            GUIStyle style = new GUIStyle() { fontSize = 10 };
            foreach (Node node in allMap)
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
        public void Init(Vector2Int urbanCenterNode)
        {
            allMap = new List<Node>();
            
            for (int i = 0; i < mapSize.y; i++)
            {
                for (int j = 0; j < mapSize.x; j++)
                {
                    int randomChanceOfObstacle = Random.Range(0,100);
                    
                    SpriteRenderer newTile = Instantiate(prefabTile, new Vector2(j, i), Quaternion.identity);

                    if(randomChanceOfObstacle < 25)
                    {
                        if(allMap.Count > 1 && new Vector2Int(j, i) != urbanCenterNode)
                        {
                            allMap.Add(new Node(new Vector2Int(j, i), true));

                            SpriteRenderer underTile = Instantiate(prefabTile, new Vector2(j, i), Quaternion.identity);
                            underTile.sprite = walkeableTile;
                            underTile.sortingOrder = 0;

                            newTile.sprite = blockedTile;
                            newTile.sortingOrder = 2;
                        }
                        else
                        {
                            allMap.Add(new Node(new Vector2Int(j, i)));
                            newTile.sprite = walkeableTile;
                        }
                    }
                    else
                    {
                        allMap.Add(new Node(new Vector2Int(j, i)));
                        newTile.sprite = walkeableTile;
                    }
                }
            }

            Node urbanCenter = allMap.Find(node => node.GetCellPosition() == urbanCenterNode);
            urbanCenter.SetLocked(false);

            pathfinding = new Pathfinding(allMap);

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            mainCamera.transform.position = new Vector3((mapSize.x * 0.5f), (mapSize.y * 0.5f), -10);

            mapInitialized = true;
        }

        public List<Vector2Int> GetPath(Vector2Int origin, Vector2Int destination)
        {
            if(!mapInitialized)
            {
                return null;
            }
            Debug.Log("GIVE ME A PATH, MAP CONTROLLER");

            Node originNode = WalkeableMap.Find(node => node.GetCellPosition() == new Vector2Int(origin.x, origin.y));
            Node destinationNode = WalkeableMap.Find(node => node.GetCellPosition() == new Vector2Int(destination.x, destination.y));

            return pathfinding.GetPath(originNode.GetCellPosition(), destinationNode.GetCellPosition());
        }

        public Vector2Int GetRandomMapPosition()
        {
            return GetRandomMapNode().GetCellPosition();
        }

        public Node GetMapNodeFromPosition(Vector2Int position)
        {
            return WalkeableMap.Find(node => node.GetCellPosition() == position);
        }

        public Node GetRandomMapNode()
        {
            int randomIndex = Random.Range(0, WalkeableMap.Count);
            Vector2Int randomPosition = WalkeableMap[randomIndex].GetCellPosition();

            return GetMapNodeFromPosition(randomPosition);
        }
        #endregion
    }
}