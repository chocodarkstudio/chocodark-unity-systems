using System;
using UnityEngine;

namespace ChocoDark
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [field: Header("Inner Pivot")]
        [Tooltip("This can be the camera (as a child)")]
        [SerializeField] Transform innerPivot;
        [field: SerializeField] public float MaxInnerPivotDistance { get; set; } = 4f;
        public float InnerPivotDistance { get; private set; }
        [SerializeField] Vector3 innerPivotOffset;

        [field: Header("Follow")]
        [field: SerializeField] public Transform FollowTarget { get; set; }
        [field: SerializeField] public float FollowSpeed { get; set; } = 5f;

        [field: Header("Mouse Rotation")]
        [field: SerializeField] public bool EnableMouseRotation { get; set; } = true;
        [field: SerializeField] public float MouseSensivility { get; set; } = 1;
        [field: SerializeField] public bool LockMouse { get; set; } = true;

        [Header("Debug")]
        [SerializeField] bool visualizations;
        [SerializeField] bool visualizeCameraTriangle;

        // cache
        Vector3 innerPivotTriangle;

        // non-alloc
        readonly RaycastHit[] hits = new RaycastHit[3];

        private void OnValidate()
        {
            // only editor mode and not playing
            if (Application.isPlaying)
                return;

            SetInnerPivotDistance(MaxInnerPivotDistance);
        }

        void Update()
        {
            if (FollowTarget != null)
            {
                Vector3 distance = FollowTarget.position - transform.position;
                transform.position += distance * FollowSpeed * Time.deltaTime;
            }

            if (EnableMouseRotation)
            {
                if (LockMouse)
                    Cursor.lockState = CursorLockMode.Locked;
                Vector3 inputRotation = new(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0);
                RotateByDelta(inputRotation*MouseSensivility);
            }
            else if (LockMouse)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            UpdateObstacleDistance();
        }

        public void RotateByDelta(Vector3 deltaRotation)
        {
            transform.Rotate(deltaRotation);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }

        void UpdateObstacleDistance()
        {
            // Create the ray based on the vertex opposite the right angle of the hypotenuse in the camera triangle
            Vector3 origin = transform.TransformPoint(innerPivot.localPosition - innerPivotTriangle);
            Vector3 direction = origin - innerPivot.position;
            Ray ray = new(origin, -direction.normalized);

            if (visualizations)
            {
                //DebugExtension.DebugPoint(origin);
                DebugExtension.DebugArrow(origin, ray.direction * MaxInnerPivotDistance, Color.yellow);
            }


            int count = Physics.RaycastNonAlloc(ray, hits, MaxInnerPivotDistance);

            // If there is no obstacle, it will be 'maxCameraDistance' by default
            float minDistance = MaxInnerPivotDistance;

            // search the nearest obstacle
            for (int i = 0; i < count; i++)
            {
                RaycastHit hit = hits[i];
                // ignore follow target
                if (hit.transform == FollowTarget)
                    continue;

                if (visualizations)
                    DebugExtension.DebugWireSphere(hit.point, Color.red, 0.2f);

                // record the minimum distance
                if (hit.distance < minDistance)
                    minDistance = hit.distance;
            }
            SetInnerPivotDistance(minDistance);
        }

        float DegreesToRadians(float degrees)
            => degrees * (MathF.PI / 180);

        /// <summary>
        /// Calculate camera right triangle sides with a given hypotenuse </summary>
        void CalculateTriangleSides(float hypotenuse, out float opposite, out float adyacent)
        {
            float angle = innerPivot.localEulerAngles.x;
            float angleR = DegreesToRadians(angle);

            opposite = hypotenuse * MathF.Sin(angleR);
            adyacent = hypotenuse * MathF.Cos(angleR);
        }


        /// <summary>
        /// Sets the Camera position distance by adjusting the hypotenuse length of the camera triangle </summary>
        void SetInnerPivotDistance(float distance)
        {
            InnerPivotDistance = distance;

            // camera has to move back (negative distance)
            CalculateTriangleSides(-distance, out float opposite, out float adyacent);

            // cache, localSpace
            innerPivotTriangle = new Vector3(0, -opposite, adyacent);

            innerPivot.localPosition = innerPivotTriangle + innerPivotOffset;
        }

        public Vector3 ProjectOnForward(Vector3 localDirection)
        {
            Vector3 forward = transform.TransformDirection(localDirection);
            forward.y = 0;
            return forward;
        }

        void DrawCameraTriangleGizmos()
        {
            float opposite = innerPivotTriangle.y;
            float adjacent = innerPivotTriangle.z;

            // Define the positions of the triangle vertices in local space
            Vector3 vertexA = innerPivot.position;
            Vector3 vertexB = vertexA + transform.TransformDirection(new Vector3(0, 0, -adjacent));
            Vector3 vertexC = vertexA + transform.TransformDirection(new Vector3(0, -opposite, -adjacent));

            // adjacent
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(vertexA, vertexB);

            // opposite
            Gizmos.color = Color.red;
            Gizmos.DrawLine(vertexB, vertexC);

            // hypotenuse
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(vertexC, vertexA);

            // Fill the triangle using a Mesh
            Mesh triangleMesh = new()
            {
                vertices = new Vector3[] { vertexA, vertexB, vertexC, vertexA, vertexC, vertexB },
                triangles = new int[] { 0, 1, 2, 3, 4, 5 } // Second set of indices flips the triangle
            };
            triangleMesh.RecalculateNormals();

            Gizmos.color = new Color(0, 1, 0, 0.1f);
            Gizmos.DrawMesh(triangleMesh);
        }

        void OnDrawGizmos()
        {
            if (visualizeCameraTriangle)
                DrawCameraTriangleGizmos();
        }

    }

}

