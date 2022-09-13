using System.Collections.Generic;

using UnityEngine;

using PrimerParcial.Gameplay.Entities;

namespace PrimerParcial.Gameplay.Controllers
{
    public class UrbanCenterController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Mine prefabMine = null;
        [SerializeField] private int nMinesAmount = 10;
        [SerializeField] private MapController mapHandler = default;
        #endregion

        #region PRIVATE_FIELDS
        private List<Vector2Int> selectedPositions = new List<Vector2Int>();
        private List<Mine> mines = new List<Mine>();
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

                selectedPositions.Add(randomPosition);

                Mine mine = Instantiate(prefabMine, (Vector2)randomPosition, Quaternion.identity);
                
                if(!mines.Contains(mine))
                {
                    mines.Add(mine);
                }
            }
        }

        private void Update()
        {
            
        }
        #endregion
    }
}