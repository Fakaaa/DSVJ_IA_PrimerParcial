using System.Linq;
using System.Collections.Generic;
using PrimerParcial.Common.Map.Utils;
using UnityEngine;

using PrimerParcial.Gameplay.Entities;
using PrimerParcial.Gameplay.Voronoi.Handler;
using PrimerParcial.Gameplay.Controllers.View;

namespace PrimerParcial.Gameplay.Controllers
{
    public class UrbanCenterController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Miner prefabMiner = null;
        [SerializeField] private Mine prefabMine = null;
        [SerializeField] private int amountOrePerMine = 50;
        [SerializeField] private int nMinesAmount = 10;
        [SerializeField] private MapController mapHandler = default;
        [SerializeField] private UIController uiController = null;
        [SerializeField] private VoronoiHandler voronoiHandler = null;
        #endregion

        #region PRIVATE_FIELDS
        private List<Vector2> selectedPositions = new List<Vector2>();
        private List<Mine> mines = new List<Mine>();

        private List<Miner> allMiners = new List<Miner>();

        private UrbanCenter urbanCenter = null;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            transform.position = new Vector3((mapHandler.MapSize.x * 0.5f), (mapHandler.MapSize.y * 0.5f), 0.0f);
            Vector2 urbanCenterPos = transform.position;
            
            mapHandler.Init(urbanCenterPos); 
            MapUtils.SetMapSize(mapHandler.MapSize);
             
            voronoiHandler.Initialize();

            uiController.ConfigureMinerOptions(CreateNewMiner, EnableAllMiners, RemoveMiner, ClearAllMiners,SendAllMinersToUrbanCenter, SendAllMinersToWorkAgain);
            
            Node urbanCenterNode = mapHandler.GetMapCenterNode();
            transform.position = new Vector2(urbanCenterPos.x, urbanCenterPos.y);
            urbanCenter = new UrbanCenter(urbanCenterNode);

            selectedPositions.Add(urbanCenterNode.GetCellPosition());

            for (int i = 0; i < nMinesAmount; i++)
            {
                Vector2 randomPosition = default;
                do
                {
                    randomPosition = mapHandler.GetRandomMapPosition();

                } while (selectedPositions.Contains(randomPosition));

                Node mineNode = mapHandler.GetMapNodeFromPosition(randomPosition);

                selectedPositions.Add(randomPosition);

                mineNode.SetLocked(false);

                Mine mine = Instantiate(prefabMine, randomPosition, Quaternion.identity);
                
                if(!mines.Contains(mine))
                {
                    mine.Init(mineNode, amountOrePerMine, RevalidateVoronoi);
                    mines.Add(mine);
                    voronoiHandler.ValidateVoronoi(mines);
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

        private void RevalidateVoronoi(Mine mine)
        {
            if (mines.Contains(mine))
            {
                mines.Remove(mine);
            }
            voronoiHandler.ValidateVoronoi(mines);
        }
        
        private void CreateNewMiner()
        {
            int randomIndex = Random.Range(0, mapHandler.WalkeableMap.Count);

            Node whereSpawnMiner = mapHandler.WalkeableMap[randomIndex];

            Miner newMiner = Instantiate(prefabMiner, new Vector2(whereSpawnMiner.GetCellPosition().x, whereSpawnMiner.GetCellPosition().y), Quaternion.identity);

            newMiner.OnGetPathOnMap += mapHandler.GetPath;
            newMiner.Init(mines, urbanCenter, voronoiHandler.GetClosestMine);

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

        private void SendAllMinersToUrbanCenter()
        {
            for (int i = 0; i < allMiners.Count; i++)
            {
                if (allMiners[i] != null)
                {
                    allMiners[i].EscapeFromDanger();
                }
            }
        }
        
        private void SendAllMinersToWorkAgain()
        {
            for (int i = 0; i < allMiners.Count; i++)
            {
                if (allMiners[i] != null)
                {
                    allMiners[i].ContinueWork();
                }
            }
        }
        #endregion
    }

    [System.Serializable]
    public class UrbanCenter
    {
        public Node attachedNode = null;

        public UrbanCenter(Node node)
        {
            attachedNode = node;
            attachedNode.SetLocked(false);
        }
    }
}