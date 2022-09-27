using System;

using UnityEngine;
using UnityEngine.UI;

namespace PrimerParcial.Gameplay.Controllers.View
{
    public class UIController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [Header("MINERS")]
        [SerializeField] private Button btnAddMiner = null;
        [SerializeField] private Button btnEnableMiners = null;
        [SerializeField] private Button btnRemoveMiner = null;
        [SerializeField] private Button btnClearAllMiner = null;
        #endregion

        #region PUBLIC_METHODS
        public void ConfigureMinerOptions(Action onAddMiner, Action enableAllMiners,Action onRemoveMiner, Action onClearAllMiners)
        {
            btnAddMiner.onClick.AddListener(() => { onAddMiner?.Invoke(); });
            btnEnableMiners.onClick.AddListener(() => { enableAllMiners?.Invoke(); });
            btnRemoveMiner.onClick.AddListener(() => { onRemoveMiner?.Invoke(); });
            btnClearAllMiner.onClick.AddListener(() => { onClearAllMiners?.Invoke(); });
        }
        #endregion
    }
}