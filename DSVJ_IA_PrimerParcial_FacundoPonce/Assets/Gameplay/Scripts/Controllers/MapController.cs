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
        public Vector2Int MapSize => mapSize;
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
        public void Init(Vector2 urbanCenterNode)
        {
            allMap = new List<Node>();
            
            for (int i = 0; i < mapSize.y; i++)
            {
                for (int j = 0; j < mapSize.x; j++)
                {
                    int randomChanceOfObstacle = Random.Range(0,100);

                    Vector2 position = new Vector2(j + 0.5f, i + 0.5f);
                    SpriteRenderer newTile = Instantiate(prefabTile, position, Quaternion.identity);

                    if(randomChanceOfObstacle < 25)
                    {
                        if(allMap.Count > 1 && position != urbanCenterNode)
                        {
                            allMap.Add(new Node(position, new Vector2Int(j, i),true));

                            SpriteRenderer underTile = Instantiate(prefabTile,position, Quaternion.identity);
                            underTile.sprite = walkeableTile;
                            underTile.sortingOrder = 0;

                            newTile.sprite = blockedTile;
                            newTile.sortingOrder = 2;
                        }
                        else
                        {
                            allMap.Add(new Node(position, new Vector2Int(j, i)));
                            newTile.sprite = walkeableTile;
                        }
                    }
                    else
                    {
                        allMap.Add(new Node(position, new Vector2Int(j, i)));
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

        public List<Vector2> GetPath(Vector2 origin, Vector2 destination)
        {
            if(!mapInitialized)
            {
                return null;
            }
            Debug.Log("GIVE ME A PATH, MAP CONTROLLER");

            origin = new Vector2(Mathf.Abs(origin.x),Mathf.Abs( origin.y));
            
            Node originNode = WalkeableMap.Find(node => node.GetCellPosition() == new Vector2(origin.x, origin.y));
            Node destinationNode = WalkeableMap.Find(node => node.GetCellPosition() == new Vector2(destination.x, destination.y));

            return pathfinding.GetPath(originNode.GetCellPosition(), destinationNode.GetCellPosition());
        }

        public Vector2 GetRandomMapPosition()
        {
            return GetRandomMapNode().GetCellPosition();
        }

        public bool IsNodeOnMapLimit(Node node)
        {
            return (node.GridPosition.x == 0 || node.GridPosition.x == mapSize.y -1) || (node.GridPosition.y == 0 || node.GridPosition.y == mapSize.x -1);
        }
        
        public Node GetMapNodeFromPosition(Vector2 position)
        {
            return WalkeableMap.Find(node => node.GetCellPosition() == position);
        }

        public Node GetRandomMapNode()
        {
            int randomIndex = Random.Range(0, WalkeableMap.Count);
            Node node = WalkeableMap[randomIndex];

            if (IsNodeOnMapLimit(node))
            {
                return GetRandomMapNode();
            }
            
            return node;
        }

        public Node GetMapCenterNode()
        {
            return WalkeableMap.Find(node => (node.GridPosition.x == (int)(mapSize.y * 0.5f)) && (node.GridPosition.y == (int)(mapSize.x * 0.5f)));
        }
        #endregion
    }
}