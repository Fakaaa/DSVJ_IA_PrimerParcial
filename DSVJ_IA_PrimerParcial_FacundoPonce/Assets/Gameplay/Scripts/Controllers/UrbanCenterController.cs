using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using PrimerParcial.Gameplay.Entities;
using PrimerParcial.Gameplay.Controllers.View;

namespace PrimerParcial.Gameplay.Controllers
{
    public class UrbanCenterController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Miner prefabMiner = null;
        [SerializeField] private Mine prefabMine = null;
        [SerializeField] private int nMinesAmount = 10;
        [SerializeField] private MapController mapHandler = default;
        [SerializeField] private UIController uiController = null;
        #endregion

        #region PRIVATE_FIELDS
        private List<Vector2Int> selectedPositions = new List<Vector2Int>();
        private List<Mine> mines = new List<Mine>();

        private List<Miner> allMiners = new List<Miner>();

        private UrbanCenter urbanCenter = null;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            mapHandler.Init();

            uiController.ConfigureMinerOptions(CreateNewMiner, EnableAllMiners, RemoveMiner, ClearAllMiners);

            transform.position = new Vector3((mapHandler.MapSize.x * 0.5f), (mapHandler.MapSize.y * 0.5f), 0.0f);
            
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

            selectedPositions.Add(nearestNodes.GetCellPosition());

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
        }

        private void Update()
        {
            if(allMiners.Count < 1)
            {
                return;
            }

            if(allMiners.Any())
            {
                foreach (Miner miner in allMiners)
                {
                    miner.UpdateMiner();
                }
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void CreateNewMiner()
        {
            List<Node> freeNodes = mapHandler.Map.FindAll(node=> !node.IsLocked);
            int randomIndex = Random.Range(0, freeNodes.Count);

            Node whereSpawnMiner = freeNodes[randomIndex];

            Miner newMiner = Instantiate(prefabMiner, new Vector2(whereSpawnMiner.GetCellPosition().x, whereSpawnMiner.GetCellPosition().y), Quaternion.identity);

            newMiner.OnGetPathOnMap += mapHandler.GetPath;
            newMiner.Init(mines, urbanCenter);

            allMiners.Add(newMiner);
        }

        private void RemoveMiner()
        {
            int countMiners = allMiners.Count;

            if(allMiners.Count > 1)
            {
                Miner firstMiner = allMiners[0];

                firstMiner.DestroyMiner();

                allMiners.Remove(firstMiner);
            }
        }

        private void EnableAllMiners()
        {
            for (int i = 0; i < allMiners.Count; i++)
            {
                if (allMiners[i] != null)
                {
                    allMiners[i].ToggleBehaviour();
                }
            }
        }

        private void ClearAllMiners()
        {
            for (int i = 0; i < allMiners.Count; i++)
            {
                if (allMiners[i] != null)
                {
                    allMiners[i].DestroyMiner();
                }
            }

            allMiners.Clear();
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