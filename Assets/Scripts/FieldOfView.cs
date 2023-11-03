using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;

    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetLayerMask;
    public LayerMask obstacleMask;

    [SerializeField]
    private List<Transform> _visibleTargets = new List<Transform>();

    [SerializeField]
    private float _meshResolution; // Determines how many rays we will cast out
    [SerializeField]
    private MeshFilter _viewMeshFilter;
    private Mesh _viewMesh;

    [SerializeField]
    private int _edgeResolveIterations;
    [SerializeField]
    private float _edgeDistanceThreshold; // For sceanrio where the rays hit 2 different targets
    public List<Transform> GetVisibleTargets()
    {
        return _visibleTargets;
    }

    private void Start()
    {
        _viewMesh = new Mesh();
        _viewMesh.name = "Line Of Sight";
        _viewMeshFilter.mesh = _viewMesh;

        StartCoroutine(nameof(FindTargetsWithDelay), 0.2f);
    }

    private void LateUpdate()
    {
        // Called Here to allow rigibody to complete rotation
        DrawFieldOfView();
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        _visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetLayerMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)// Within Radius
            {
                // Use Raycast to check if it is in LOS (Line of Sight) i.e., not blocked by an obstacle
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    _visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * _meshResolution);
        float stepAngleSize = viewAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastData oldViewCast = new ViewCastData();

        // Covering 
        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;

            //Debug.DrawLine(transform.position, transform.position + DirectionFromAngle(angle, true) * viewRadius, Color.cyan);

            ViewCastData newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.endPoint);

            // View cast won't be set at i = 0
            //if (i > 0)
            //{
            //    bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            //    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
            //    {
            //        EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
            //        if (edge.pointA != Vector3.zero)
            //        {
            //            viewPoints.Add(edge.pointA);
            //        }
            //        if (edge.pointB != Vector3.zero)
            //        {
            //            viewPoints.Add(edge.pointB);
            //        }
            //    }

            //}
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            // Local To Global
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _viewMesh.Clear();

        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }

    EdgeCastData FindEdge(ViewCastData minViewCast, ViewCastData maxViewCast)
    {
        float minAngle = minViewCast.angleOfRay;
        float maxAngle = maxViewCast.angleOfRay;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastData newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.lengthOfRay - newViewCast.lengthOfRay) > _edgeDistanceThreshold;
            if (newViewCast.hasHit == minViewCast.hasHit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.endPoint;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.endPoint;
            }
        }

        return new EdgeCastData(minPoint, maxPoint);
    }

    ViewCastData ViewCast(float globalAngle)
    {
        Vector3 direction = DirectionFromAngle(globalAngle, true);
        RaycastHit raycastHit;

        if (Physics.Raycast(transform.position, direction, out raycastHit, viewRadius, obstacleMask))
        {
            return new ViewCastData(true, raycastHit.point, raycastHit.distance, globalAngle);
        }
        else
        {
            return new ViewCastData(false, transform.position + direction * viewRadius, viewRadius, globalAngle);
        }
    }

    // For Raycast
    public struct ViewCastData
    {
        public bool hasHit;
        public Vector3 endPoint;
        public float lengthOfRay;
        public float angleOfRay;

        public ViewCastData(bool _hasHit, Vector3 _endPoint, float _lengthOfRay, float _angleOfRay)
        {
            hasHit = _hasHit;
            endPoint = _endPoint;
            lengthOfRay = _lengthOfRay;
            angleOfRay = _angleOfRay;
        }
    }

    // For Fixing the Edge Problem in Raycast
    public struct EdgeCastData
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeCastData(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
