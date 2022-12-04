using System;
using UnityEngine;

using PrimerParcial.Gameplay.Interfaces;
using TMPro;

namespace PrimerParcial.Gameplay.Entities
{
    public class Mine : MonoBehaviour, IMinable
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TMP_Text txtOreAmount = null;
        [SerializeField] private int amountOre = 50;
        #endregion

        #region PRIVATE_FIELDS
        private int maxAmountOre = 0;
        private float delayToTurnTxt = 5f;
        private float timer = 0f;
        private Node assignedNode = null;
        private Action<Mine> onMineEmpty = null;
        #endregion

        #region PROPERTIES
        public Vector2 Position { get { return assignedNode.GetCellPosition(); } }
        public bool IsEmpty { get { return amountOre <= 0; } }
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            if(timer < delayToTurnTxt)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = delayToTurnTxt;

                if (txtOreAmount.gameObject.activeSelf)
                {
                    txtOreAmount.gameObject.SetActive(false);
                }
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Node assignedNode, int amountOre, Action<Mine> onMineEmpty)
        {
            this.onMineEmpty = onMineEmpty;
            this.assignedNode = assignedNode;

            this.amountOre = amountOre;
            maxAmountOre = amountOre;

            UpdateText();

            if (txtOreAmount.gameObject.activeSelf)
            {
                txtOreAmount.gameObject.SetActive(false);
            }
        }

        public Vector2 GetMinePosition()
        {
            return assignedNode.GetCellPosition();
        }

        public bool OnMine(int amountMining)
        {
            if (amountOre <= 0)
            {                
                txtOreAmount.gameObject.SetActive(false);
                onMineEmpty?.Invoke(this);
                Destroy(gameObject);
                return false;
            }

            if(!txtOreAmount.gameObject.activeSelf)
            {
                txtOreAmount.gameObject.SetActive(true);
                timer = 0;
            }

            if (amountMining < amountOre)
            {
                amountOre -= amountMining;
            }
            else
            {
                amountOre = 0;
            }

            UpdateText();

            if (amountOre < 0)
            {
                amountOre = 0;
            }

            return true;
        }
        #endregion

        #region PRIVATE_METHODS
        private void UpdateText()
        {
            if(amountOre > (maxAmountOre * 0.5f))
            {
                txtOreAmount.text = "<color=green>Ore: " + amountOre + "</color>";
            }
            else if(amountOre < (maxAmountOre * 0.5f) && amountOre > 2000)
            {
                txtOreAmount.text = "<color=yellow>Ore: " + amountOre + "</color>";
            }
            else if(amountOre < 2000)
            {
                txtOreAmount.text = "<color=red>Ore: " + amountOre + "</color>";
            }
        }
        #endregion
    }
}