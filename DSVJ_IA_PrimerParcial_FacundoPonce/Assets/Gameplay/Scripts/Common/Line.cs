using UnityEngine;

namespace PrimerParcial.Gameplay.Voronoi.Utils
{
    [System.Serializable]
    public class Line
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Vector2 start;
        [SerializeField] private Vector2 end;
        [SerializeField] private Vector2 middlePoint;
        #endregion

        #region PROPERTIES
        public Vector2 Start { get { return start; } }
        public Vector2 End { get { return end; } }
        public Vector2 MiddlePoint { get { return middlePoint; } }
        #endregion

        #region CONSTRUCTOR
        public Line(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;

            GetMiddlePoint();
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetLine(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;

            GetMiddlePoint();
        }
        #endregion

        #region PRIVATE_METHODS
        private void GetMiddlePoint()
        {
            middlePoint = new Vector2((start.x + end.x) * 0.5f, (start.y + end.y) * 0.5f);
        }
        #endregion
    }
}