namespace Entity
{
    using System.Collections;

    using UnityEngine;

    public class Enemy : MonoBehaviour
    {
        // Event - Delegate with no parameters and void return
        public static event System.Action OnGuardHasSpottedPlayer;

        [SerializeField]
        private float _speed = 5f;

        [SerializeField]
        private float _waitTime = 0.3f;

        [SerializeField]
        private float _turnSpeed = 90f; // 90 degrees per second

        [SerializeField]
        private Transform _pathHolder;


        [SerializeField]
        private Light _spotLight;

        [SerializeField]
        private float _viewDistance;

        private float _viewAngle;
        private Color _originalSpotlightColor;

        [SerializeField]
        private float _timeToSpotPlayer;
        private float _playerVisibleTimer;


        [SerializeField]
        private Transform _player;


        [SerializeField]
        private LayerMask _viewMask;


        private void Awake()
        {
            if (_spotLight == null)
            {
                _spotLight = GetComponentInChildren<Light>();
            }

            _viewAngle = _spotLight.spotAngle;
            _originalSpotlightColor = _spotLight.color;

            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Start()
        {
            Vector3[] waypoints = new Vector3[_pathHolder.childCount];

            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = _pathHolder.GetChild(i).position;
                waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
            }

            StartCoroutine(FollowPath(waypoints));
        }


        private void Update()
        {
            if (CanSeePlayer())
            {
                _playerVisibleTimer += Time.deltaTime;
                //_spotLight.color = Color.red;
            }
            else
            {
                _playerVisibleTimer -= Time.deltaTime;
            }

            _playerVisibleTimer = Mathf.Clamp(_playerVisibleTimer, 0, _timeToSpotPlayer);
            _spotLight.color = Color.Lerp(_originalSpotlightColor, Color.red, _playerVisibleTimer / _timeToSpotPlayer);

            if (_playerVisibleTimer >= _timeToSpotPlayer)
            {
                if (OnGuardHasSpottedPlayer != null)
                {
                    OnGuardHasSpottedPlayer();
                }
            }
        }

        private bool CanSeePlayer()
        {
            if (Vector3.Distance(transform.position, _player.position) < _viewDistance)
            {
                Vector3 directionToPlayer = (_player.position - transform.position).normalized;

                float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleBetweenGuardAndPlayer < _viewAngle / 2f) // Smallest angle
                {
                    // Raycast
                    if (!Physics.Linecast(transform.position, _player.position, _viewMask))
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        IEnumerator FollowPath(Vector3[] waypoints)
        {
            transform.position = waypoints[0];

            int targetWaypointIndex = 1;
            Vector3 targetWaypoint = waypoints[targetWaypointIndex];

            while (true)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, _speed * Time.deltaTime);

                if (transform.position == targetWaypoint)
                {
                    targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                    targetWaypoint = waypoints[targetWaypointIndex];

                    yield return new WaitForSeconds(_waitTime);
                    yield return StartCoroutine(TurnToFace(targetWaypoint));
                }

                yield return null;
            }
        }

        private IEnumerator TurnToFace(Vector3 lookTarget)
        {
            Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
            float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

            while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
            {
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, _turnSpeed * Time.deltaTime);
                transform.eulerAngles = Vector3.up * angle;
                yield return null;
            }
        }

        #region Editor
        private void OnDrawGizmos()
        {
            if (_pathHolder != null && _pathHolder.childCount > 0)
            {
                Vector3 startPosition = _pathHolder.GetChild(0).position;
                Vector3 previousPosition = startPosition;

                foreach (Transform waypoint in _pathHolder)
                {
                    Gizmos.DrawSphere(waypoint.position, 0.3f);

                    Gizmos.DrawLine(previousPosition, waypoint.position);
                    previousPosition = waypoint.position;
                }

                Gizmos.DrawLine(previousPosition, startPosition);
            }

            Gizmos.color = Color.red;

            Gizmos.DrawRay(transform.position, transform.forward * _viewDistance);
        }
        #endregion
    }
}