using UnityEngine;

using PrimerParcial.Gameplay.Map.Data;

namespace PrimerParcial.Gameplay.Entities
{
    public class Mine : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private int amountOre = 10;
        #endregion

        #region PRIVATE_FIELDS
        private MapNode assignedNode = null;
        #endregion

        #region PUBLIC_METHODS
        public void Init(MapNode assignedNode)
        {
            this.assignedNode = assignedNode;
        }

        public Vector2Int GetMinePosition()
        {
            return assignedNode.position;
        }
        #endregion
    }
}