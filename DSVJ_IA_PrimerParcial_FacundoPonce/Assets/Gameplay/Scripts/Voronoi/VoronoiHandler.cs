using System.Collections.Generic;

using PrimerParcial.Common.Map.Utils;
using PrimerParcial.Gameplay.Entities;
using PrimerParcial.Gameplay.Voronoi.Utils;

using UnityEngine;

namespace PrimerParcial.Gameplay.Voronoi.Handler
{
    public class VoronoiHandler : MonoBehaviour
    {
        #region PRIVATE_FIELDS
        private List<Limit> limits = null;
        private List<Zone> zones = null;
        #endregion

        #region UNITY_CALLS
        private void OnDrawGizmos()
        {
            DrawThiessenDiagramme();
        }
        #endregion
        
        #region PUBLIC_METHODS
        public void Initialize()
        {
            zones = new List<Zone>();
            
            InitializeLimits();
        }

        public void ValidateVoronoi(List<Mine> allMines)
        {
            zones.Clear();

            if(allMines.Count == 0)
                return;

            for (int i = 0; i < allMines.Count; i++)
            {
                if (allMines[i] != null)
                {
                    zones.Add(new Zone(allMines[i]));
                }
            }

            for (int i = 0; i < zones.Count; i++)
            {
                if (zones[i] != null)
                {
                    zones[i].AddLineLimits(limits);
                }
            }

            for (int i = 0; i < allMines.Count; i++)
            {
                for (int j = 0; j < allMines.Count; j++)
                {
                    if (i == j)
                        continue;
                    
                    zones[i].AddLine(allMines[i].Position, allMines[j].Position);
                }
            }

            for (int i = 0; i < zones.Count; i++)
            {
                if (zones[i] != null)
                {
                    zones[i].SetIntersections();
                }
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void InitializeLimits()
        {
            limits = new List<Limit>();
            
            limits.Add(new Limit(new Vector2(-0.5f,-0.5f), Direction.LEFT));
            limits.Add(new Limit(new Vector2(-0.5f, MapUtils.MapSize.y), Direction.UP));
            limits.Add(new Limit(new Vector2(MapUtils.MapSize.x, MapUtils.MapSize.y), Direction.RIGHT));
            limits.Add(new Limit(new Vector2(MapUtils.MapSize.x, -0.5f), Direction.DOWN));
        }
        
        private void DrawThiessenDiagramme()
        {
            if (zones == null)
                return;

            DrawLimits();
            
            for (int i = 0; i < zones.Count; i++)
            {
                if (zones[i] != null)
                {
                    Gizmos.color = Color.cyan;
                    zones[i].DrawVoronoiZone();
                    zones[i].DrawLines();
                }
            }
        }

        private void DrawLimits()
        {
            if(limits.Count == 0)
                return;

            for (int i = 0; i < limits.Count; i++)
            {
                if (i + 1 < limits.Count)
                {
                    limits[i].DrawLimit(limits[i+1].Origin, Color.red);
                }
            }
            
            limits[limits.Count-1].DrawLimit(limits[0].Origin,Color.red);
        }
        #endregion
    }
}
