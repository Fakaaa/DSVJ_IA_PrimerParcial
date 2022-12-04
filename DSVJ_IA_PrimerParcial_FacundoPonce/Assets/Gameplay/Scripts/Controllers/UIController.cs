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
        [SerializeField] private Button btnSendAlert = null;
        [SerializeField] private Button btnContinueWork = null;
        #endregion

        #region PUBLIC_METHODS
        public void ConfigureMinerOptions(Action onAddMiner, Action enableAllMiners,Action onRemoveMiner, Action onClearAllMiners, Action onSendAlert, Action continueWork)
        {
            btnAddMiner.onClick.AddListener(() => { onAddMiner?.Invoke(); });
            btnEnableMiners.onClick.AddListener(() => { enableAllMiners?.Invoke(); });
            btnRemoveMiner.onClick.AddListener(() => { onRemoveMiner?.Invoke(); });
            btnClearAllMiner.onClick.AddListener(() => { onClearAllMiners?.Invoke(); });

            btnContinueWork.interactable = false;
            
            btnSendAlert.onClick.AddListener(
                () =>
                {
                    onSendAlert?.Invoke();
                    btnSendAlert.interactable = false;
                    btnContinueWork.interactable = true;
                });
            btnContinueWork.onClick.AddListener(
                () =>
                {
                    continueWork?.Invoke();
                    btnContinueWork.interactable = false;
                    btnSendAlert.interactable = true;
                });
        }
        #endregion
    }
}