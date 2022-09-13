using UnityEngine;

namespace PrimerParcial.Gameplay.Controllers
{
    public class UrbanCenterController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private GameObject prefabMine = null;
        [SerializeField] private int nMinesAmount = 10;
        [SerializeField] private MapController mapHandler = default;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            mapHandler.Init();

            transform.position = new Vector3((mapHandler.MapSize.x * 0.5f), (mapHandler.MapSize.y * 0.5f), 0.0f);

            for (int i = 0; i < nMinesAmount; i++)
            {
                GameObject randomMine = Instantiate(prefabMine, (Vector2)mapHandler.GetRandomMapLocation().position, Quaternion.identity);
            }
        }

        private void Update()
        {
            
        }
        #endregion
    }
}