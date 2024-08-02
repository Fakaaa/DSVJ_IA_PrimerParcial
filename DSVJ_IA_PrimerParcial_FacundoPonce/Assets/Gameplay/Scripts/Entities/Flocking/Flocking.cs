using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Projects.AI.Flocking
{
    public class Flocking : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        public LayerMask LayerMask;
        public Vector3 baseRotation;
        public float maxSpeed;
        public float maxForce;
        public float checkRadious;

        public float separationMultiplayer;
        public float cohesionMultiplayer;
        public float aligmentMultiplayer;

        public Vector2 velocity;
        public Vector2 aceleration;
        #endregion

        #region PRIVATE_FIELDS
        private bool followingMouse = false;

        public Transform target = null;
        #endregion

        #region PROPERPTIES
        private Vector2 Position
        {
            get
            {
                return gameObject.transform.position;
            }
            set
            {
                gameObject.transform.position = value;
            }
        }
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
            velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public void Update()
        {
            Collider2D[] otherColliders = Physics2D.OverlapCircleAll(Position, checkRadious, LayerMask);

            if (otherColliders == null) return;

            List<Flocking> boids = otherColliders.Select(others => others.GetComponent<Flocking>()).ToList();

            if (boids == null) return;

            boids.Remove(this);

            if (boids.Any())
            {                
                aceleration = Alignment(boids) * aligmentMultiplayer + Separation(boids) * separationMultiplayer + Cohesion(boids) * cohesionMultiplayer + GoDestination();
            }
            else
            {
                aceleration = Vector2.zero;
            }

            velocity += aceleration;
            velocity = LimitMagnitude(velocity, maxSpeed);
            Position += velocity * Time.deltaTime;
            float newAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, newAngle) + baseRotation).normalized, maxSpeed * Time.deltaTime);
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetTarget(Transform target, bool isMouse = false)
        {
            followingMouse = isMouse;

            if (followingMouse)
            {
                this.target = null;
                return;
            }
            this.target = target;
        }
        #endregion

        #region PRIVATE_METHODS
        private Vector2 Alignment(IEnumerable<Flocking> boids)
        {
            Vector2 velocity = Vector2.zero;

            foreach (Flocking boid in boids)
            {
                if(boid != null)
                {
                    velocity += boid.velocity;
                }
            }
            velocity /= boids.Count();

            return Steer(velocity.normalized * maxSpeed);
        }

        private Vector2 Cohesion(IEnumerable<Flocking> boids)
        {
            Vector2 sumPositions = Vector2.zero;
            foreach (Flocking boid in boids)
            {
                sumPositions += boid.Position;
            }
            Vector2 average = sumPositions / boids.Count();
            Vector2 direction = average - Position;

            return Steer(direction.normalized * maxSpeed);
        }

        private Vector2 Separation(IEnumerable<Flocking> boids)
        {
            Vector2 direction = Vector2.zero;
            boids = boids.Where(o => DistanceTo(o) <= checkRadious / 2);

            foreach (var boid in boids)
            {
                Vector2 difference = Position - boid.Position;
                direction += difference.normalized / difference.magnitude;
            }
            direction /= boids.Count();

            return Steer(direction.normalized * maxSpeed);
        }

        private Vector2 Steer(Vector2 desired)
        {
            Vector2 steer = desired - velocity;
            return LimitMagnitude(steer, maxForce);
        }

        private float DistanceTo(Flocking boid)
        {
            return Vector3.Distance(boid.transform.position, Position);
        }

        private Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
        {
            if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
            {
                baseVector = baseVector.normalized * maxMagnitude;
            }
            return baseVector;
        }

        private Vector2 GoDestination()
        {
            if(followingMouse)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                Debug.DrawRay(ray.origin, ray.direction, Color.red);

                Ray2D ray2D = new Ray2D(ray.origin, ray.direction);

                return (ray2D.origin - Position).normalized;
            }

            if(target)
            {
                return ((Vector2)target.transform.position - Position).normalized;
            }

            return Vector2.zero;
        }
        #endregion
    }
}