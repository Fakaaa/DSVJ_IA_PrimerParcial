using UnityEngine;

namespace PrimerParcial.Common.Map.Utils
{
    public static class MapUtils
    {
        public static Vector2Int MapSize = default;

        public static void SetMapSize(Vector2Int mapSize)
        {
            MapSize = mapSize;
        }
    }
}