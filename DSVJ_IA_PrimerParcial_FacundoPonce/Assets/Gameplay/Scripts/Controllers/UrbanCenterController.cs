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

            //minerPath = mapHandler.GetPath(mines[Random.Range(0, mines.Count)].GetMinePosition());

            miner.OnGetPathToMine += mapHandler.GetPath;

            miner.Init(mines, gameObject);
            /*IEnumerator MoveMinerToDestination()
            {
                if (miner == null)
                    yield break;

                if (minerPath.Any())
                {
                    foreach (Vector2Int position in minerPath)
                    {
                        while (Vector2.Distance(miner.transform.position, position) > 0.15f)
                        {
                            miner.transform.position = Vector2.MoveTowards(miner.transform.position, position, 1.15f * Time.deltaTime);

                            yield return new WaitForEndOfFrame();
                        }
                    }
                }

                yield break;
            }

            StartCoroutine(MoveMinerToDestination());*/
        }

        private void Update()
        {
            miner.UpdateMiner();
        }
        #endregion
    }
}