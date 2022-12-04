using System.Linq;
using System.Collections.Generic;

using PrimerParcial.Common.Map.Utils;
using PrimerParcial.Gameplay.Entities;
using UnityEditor;
using UnityEngine;

namespace PrimerParcial.Gameplay.Voronoi.Utils
{
    [System.Serializable]
    public class Zone
    {
        #region PRIVATE_FIELDS
        private Mine mineInside = null;
        private List<Line> lines = null;
        private List<Vector2> intersections = null;
        private Vector3[] points;

        private Color zoneColor = default;
        #endregion

        #region PROPERTIES
        public Mine MineInside => mineInside;
        #endregion

        #region CONSTRUCTORS

        public Zone(Mine mineInZone)
        {
            mineInside = mineInZone;

            lines = new List<Line>();
            intersections = new List<Vector2>();

            zoneColor = Random.ColorHSV();
            zoneColor.a = 0.55f;
        }

        #endregion

        #region PUBLIC_METHODS

        public void AddLine(Vector2 start, Vector2 end)
        {
            lines.Add(new Line(start,end));
        }

        public void DrawLines()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] != null)
                {
                    lines[i].Draw();
                }
            }
        }

        public void DrawVoronoiZone()
        {
            Handles.color = zoneColor;
            Handles.DrawAAConvexPolygon(points);
            
            Handles.color = Color.black;
            Handles.DrawPolyLine(points);
        }

        public void AddLineLimits(List<Limit> limits)
        {
            for (int i = 0; i < limits.Count; i++)
            {
                Vector2 origin = mineInside.transform.position;
                Vector2 finalPos = limits[i].GetOutsitePosition(origin);
                
                lines.Add(new Line(origin, finalPos));
            }
        }

        public void SetIntersections()
        {
            intersections.Clear();
            
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines.Count; j++)
                {
                    if(i == j)
                        continue;

                    Vector2 intersectionPoint = GetMiddlePoint(lines[i],lines[j]);

                    if (intersections.Contains(intersectionPoint))
                        continue;

                    float maxDistance = Vector2.Distance(intersectionPoint, lines[i].Start);
                    
                    bool isValidPosition = false;
                    for (int k = 0; k < lines.Count; k++)
                    {
                        if(k == i || k == j)
                            continue;

                        if (ValidateClosestPosition(intersectionPoint, lines[k].End, maxDistance))
                        {
                            isValidPosition = true;
                            break;
                        }
                    }

                    if (!isValidPosition)
                    {
                        intersections.Add(intersectionPoint);
                        lines[i].Intersections.Add(intersectionPoint);
                        lines[j].Intersections.Add(intersectionPoint);
                    }
                }
            }

            lines.RemoveAll(line => line.Intersections.Count != 2);

            SortIntersections();
            SetPointsInZone();
        }
        #endregion

        #region PRIVATE_METHODS

        private bool ValidateClosestPosition(Vector2 middlePoint, Vector2 pointEnd, float maxDistance)
        {
            float distance = Vector2.Distance(middlePoint, pointEnd);
            return distance < maxDistance;
        }

        private void SortIntersections()
        {
            List<MiddlePoint> middlePoints = new List<MiddlePoint>();
            for (int i = 0; i < intersections.Count; i++)
            {
                middlePoints.Add(new MiddlePoint(intersections[i]));
            }

            float minX = middlePoints[0].Position.x;
            float maxX = middlePoints[0].Position.x;
            float minY = middlePoints[0].Position.y;
            float maxY = middlePoints[0].Position.y;

            for (int i = 0; i < intersections.Count; i++)
            {
                if (middlePoints[i].Position.x < minX) minX = middlePoints[i].Position.x;
                if (middlePoints[i].Position.x > maxX) maxX = middlePoints[i].Position.x;
                if (middlePoints[i].Position.y < minY) minY = middlePoints[i].Position.y;
                if (middlePoints[i].Position.y > maxY) maxY = middlePoints[i].Position.y;
            }
            
            Vector2 center = new Vector2(minX + (maxX - minX) * 0.5f, minY + (maxY - minY) * 0.5f);

            for (int i = 0; i < middlePoints.Count; i++)
            {
                Vector2 pos = middlePoints[i].Position;

                middlePoints[i].Angle = Mathf.Acos((pos.x - center.x) / Mathf.Sqrt(Mathf.Pow(pos.x - center.x, 2f) + Mathf.Pow(pos.y - center.y, 2f)));

                if (pos.y > center.y)
                {
                    middlePoints[i].Angle = Mathf.PI + Mathf.PI - middlePoints[i].Angle;
                }
            }

            middlePoints = middlePoints.OrderBy(p => p.Angle).ToList();

            intersections.Clear();
            for (int i = 0; i < middlePoints.Count; i++)
            {
                intersections.Add(middlePoints[i].Position);
            }
        }
        
        private void SetPointsInZone()
        {
            points = new Vector3[intersections.Count+1];

            for (int i = 0; i < intersections.Count; i++)
            {
                if (intersections[i].x == 0.5f) //If is on the limit of the map we need rest the offset of the limit to have a correct definition of zone.
                {
                    intersections[i] = new Vector2(-0.5f, intersections[i].y);
                }
                if (intersections[i].y == 0.5f)//This also affects the Y coordinate
                {
                    intersections[i] = new Vector2(intersections[i].x, -0.5f);
                }
                points[i] = new Vector3(intersections[i].x, intersections[i].y, 0f);
            }

            points[intersections.Count] = points[0];
        }
        
        private Vector2 GetMiddlePoint(Line lineA, Line lineB)
        {
            Vector2 intersection = Vector2.zero;

            Vector2 p1 = lineA.MiddlePoint;
            Vector2 p2 = lineA.MiddlePoint + lineA.Direction * MapUtils.MapSize.magnitude;
            Vector2 p3 = lineB.MiddlePoint;
            Vector2 p4 = lineB.MiddlePoint + lineB.Direction * MapUtils.MapSize.magnitude;

            if (((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x)) == 0) return intersection;

            intersection.x = ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) / ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));
            intersection.y = ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) / ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));

            return intersection;
        }

        #endregion
    }
}