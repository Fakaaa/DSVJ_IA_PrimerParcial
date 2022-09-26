using UnityEngine;

namespace PrimerParcial.Gameplay.Entities
{
    public class Mine : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private int amountOre = 10;
        #endregion

        #region PRIVATE_FIELDS
        private Node assignedNode = null;
        #endregion

        #region PUBLIC_METHODS
        public void Init(Node assignedNode)
        {
            this.assignedNode = assignedNode;
        }

        public Vector2Int GetMinePosition()
        {
            return assignedNode.GetCellPosition();
        }
        #endregion
    }
}