using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using PrimerParcial.Gameplay.Entities;

namespace PrimerParcial.Gameplay.Controllers
{
    public class UrbanCenterController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Miner miner = null;
        [SerializeField] private GameObject prefabMiner = null;
        [SerializeField] private Mine prefabMine = null;
        [SerializeField] private int nMinesAmount = 10;
        [SerializeField] private MapController mapHandler = default;
        #endregion

        #region PRIVATE_FIELDS
        private List<Vector2Int> selectedPositions = new List<Vector2Int>();
        private List<Mine> mines = new List<Mine>();

        private UrbanCenter urbanCenter = null;

        private List<Vector2Int> minerPath = new List<Vector2Int>();
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            mapHandler.Init();

            transform.position = new Vector3((mapHandler.MapSize.x * 0.5f), (mapHandler.MapSize.y * 0.5f), 0.0f);

            for (int i = 0; i < nMinesAmount; i++)
            {
                Vector2Int randomPosition = default;
                do
                {
                    randomPosition = mapHandler.GetRandomMapPosition();

                } while (selectedPositions.Contains(randomPosition));

                Node mineNode = mapHandler.GetMapNodeFromPosition(randomPosition);

                selectedPositions.Add(randomPosition);

                mineNode.SetLocked(false);

                Mine mine = Instantiate(prefabMine, (Vector2)randomPosition, Quaternion.identity);
                
                if(!mines.Contains(mine))
                {
                    mine.Init(mineNode);
                    mines.Add(mine);
                }
            }

            Node nearestNodes = null;

            for (int i = 0; i < mapHandler.Map.Count; i++)
            {
                if (mapHandler.Map[i] != null)
                {
                    if (Mathf.Abs(Vector2Int.Distance(Vector2Int.RoundToInt(transform.position), mapHandler.Map[i].GetCellPosition())) < 0.25f)
                    {
                        nearestNodes = mapHandler.Map[i];
                    }
                }
            }

            transform.position = new Vector2(nearestNodes.GetCellPosition().x, nearestNodes.GetCellPosition().y);
            nearestNodes.SetLocked(false);
            urbanCenter = new UrbanCenter(nearestNodes);

            miner.OnGetPathOnMap += mapHandler.GetPath;

            miner.Init(mines, urbanCenter);
        }

        private void Update()
        {
            miner.UpdateMiner();
        }
        #endregion
    }

    public class UrbanCenter
    {
        public Node attachedNode = null;

        public UrbanCenter(Node node)
        {
            attachedNode = node;
        }
    }
}