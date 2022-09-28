using UnityEngine;

using PrimerParcial.Gameplay.Interfaces;

namespace PrimerParcial.Gameplay.Entities
{
    public class Mine : MonoBehaviour, IMinable
    {
        #region EXPOSED_FIELDS
        [SerializeField] private int amountOre = 50;
        #endregion

        #region PRIVATE_FIELDS
        private Node assignedNode = null;
        #endregion

        #region PROPERTIES
        public bool IsEmpty { get { return amountOre <= 0; } }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Node assignedNode, int amountOre)
        {
            this.assignedNode = assignedNode;

            this.amountOre = amountOre;
        }

        public Vector2Int GetMinePosition()
        {
            return assignedNode.GetCellPosition();
        }

        public bool OnMine(int amountMining)
        {
            if (amountOre <= 0)
                return false;

            amountOre -= amountMining;

            if (amountOre < 0)
                amountOre = 0;

            return true;
        }
        #endregion
    }
}