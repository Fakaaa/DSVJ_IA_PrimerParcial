using UnityEngine;

namespace Projects.AI.Flocking.Controllers
{
    public class CameraManager : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private bool isFocused = false;
        #endregion

        #region PROPERTIES
        public bool IsFocused 
        {
            get 
            { 
                return isFocused; 
            } 
            set 
            { 
                isFocused = value; 
            } 
        }
        #endregion

        #region PRIVATE_METHODS
        private void Awake()
        {
            mainCamera = GetComponent<Camera>();

            if (mainCamera == null)
                mainCamera.gameObject.AddComponent(typeof(Camera));
        }
        #endregion
    }
}