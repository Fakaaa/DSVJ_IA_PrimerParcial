using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Projects.AI.Flocking.Controllers
{
    public class BoidsManager : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private List<Flocking> allBoids = new List<Flocking>();
        [SerializeField] private bool isApplicationFocused = false;
        #endregion

        #region PRIVATE_FIELDS

        #endregion

        #region PROPERTIES
        public bool IsApplicationFocused { get => isApplicationFocused; set => isApplicationFocused = value; }
        #endregion

        #region PUBLIC_METHODS
        public void Init()
        {
            allBoids = FindObjectsOfType<Flocking>().ToList();
        }
        #endregion

        #region PRIVATE_METHODS
        private void HandleCursorState()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
        }

        private void OnFocusChanged(bool state)
        {
            if (allBoids.Any())
            {
                foreach(Flocking boid in allBoids)
                {
                    boid.SetTarget(null, state);
                }
            }
        }
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            Init();
        }
        #endregion
    }
}