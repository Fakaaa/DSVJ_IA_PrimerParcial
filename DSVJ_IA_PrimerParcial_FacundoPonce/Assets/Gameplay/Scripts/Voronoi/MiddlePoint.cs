using UnityEngine;

namespace PrimerParcial.Gameplay.Voronoi.Utils
{
    public class MiddlePoint
    {
        #region PRIVATE_FIELDS
        private Vector2 position = default;
        private float angle = 0f;
        #endregion

        #region PROPERTIES
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public float Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value;
            }
        }
        #endregion

        #region CONSTRUCTOR
        public MiddlePoint(Vector2 position)
        {
            this.position = position;

            angle = 0f;
        }
        #endregion
    }
}