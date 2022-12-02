using UnityEngine;

namespace PrimerParcial.Gameplay.Voronoi.Utils
{
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,

        Size
    }

    public class Limit
    {
        #region PRIVATE_FIELDS

        private Vector2 origin = default;
        private Direction direction = default;

        #endregion

        #region CONSTRUCTOR

        public Limit(Vector2 origin, Direction dir)
        {
            this.origin = origin;
            direction = dir;
        }

        #endregion

        #region PUBLIC_METHODS
        public Vector2 GetOutsitePosition(Vector2 pos)
        {
            float distanceX = Mathf.Abs(Mathf.Abs(pos.x) - Mathf.Abs(origin.x)) * 2f;
            float distanceY = Mathf.Abs(Mathf.Abs(pos.y) - Mathf.Abs(origin.y)) * 2f;

            switch (direction)
            {
                case Direction.UP:
                    pos.y += distanceY;
                    break;
                case Direction.DOWN:
                    pos.y -= distanceY;
                    break;
                case Direction.LEFT:
                    pos.x -= distanceX;
                    break;
                case Direction.RIGHT:
                    pos.x += distanceX;
                    break;
                default:
                    pos = Vector2.zero;
                    break;
            }

            return pos;
        }
        #endregion
    }
}