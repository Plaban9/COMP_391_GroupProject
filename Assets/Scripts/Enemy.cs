namespace Entity
{
    using System.Collections;

    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Experimental.GlobalIllumination;

    public class Enemy : MonoBehaviour
    {
        // Event - Delegate with no parameters and void return
        public static event System.Action OnGuardHasSpottedPlayer;

        [SerializeField]
        private float _alertSpeed = 5f;
        [SerializeField]
        private float _patrolSpeed = 5f;
        [SerializeField]
        private float _speed = 5f;
        [SerializeField]
        private float _chaseSpeed = 7.5f;

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

        [SerializeField]
        private MeshRenderer _fovRenderer;

        [SerializeField]
        private FieldOfView _fieldOfView;


        [SerializeField]
        private NavMeshAgent _navMeshAgent;

        [SerializeField]
        private float _patrolReachThreshold;

        [SerializeField]
        private float _attackThreshold;

        [SerializeField]
        private float _lastKnownReachThreshold;

        [SerializeField]
        private int _currentWaypointIndex;

        [SerializeField]
        private EnemyState _enemyState;
        private Vector3 _playerLastKnownPosition;
        [SerializeField]
        private float _searchRotateSpeed;

        private Vector3 _searchPointA;
        private Vector3 _searchPointB;

        Vector3[] waypoints;

        private float _searchStartTimer;

        [SerializeField]
        private float _searchDuartion = 3f;

        [SerializeField]
        private Color _alertColor;

        [SerializeField]
        private Color _playerSeenColor;

        [SerializeField]
        private GameObject _bullet;

        [SerializeField]
        private Transform _firePosition;

        [SerializeField]
        private float _fireRate;

        private float _lastShot;

        [SerializeField]
        private AudioClip[] _attackClips;

        [SerializeField]
        private AudioClip[] _howlClips;



        private void Awake()
        {
            if (_spotLight == null)
            {
                _spotLight = GetComponentInChildren<Light>();
            }

            _viewAngle = _spotLight.spotAngle;
            _originalSpotlightColor = _spotLight.color;

            _navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            _fieldOfView = GetComponent<FieldOfView>();
            //_fieldOfView.viewAngle = _viewAngle;
            //_fieldOfView.viewRadius = _viewDistance;
            _fovRenderer.material = Instantiate(Resources.Load("ViewVisualizationEnemy") as Material);
            ReApplyColor();
            //SetViewMaterialProperties();
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _playerLastKnownPosition = _player.position;
        }

        private void Start()
        {
            _currentWaypointIndex = 0;
            _navMeshAgent.speed = _speed;

            waypoints = new Vector3[_pathHolder.childCount];
            SetWaypoints();
            transform.position = GetWaypoints()[0];
            _enemyState = EnemyState.Patrol;
        }

        private void SetViewMaterialProperties()
        {
            //Color color = new Color(_spotLight.color.r, _spotLight.color.g, _spotLight.color.b, 0.05f);
            //_fovRenderer.material.SetFloat("_Cutoff", 0);
            //_fovRenderer.material.SetColor("_UnlitColor", color);
        }

        private void ReApplyColor()
        {
            Color color = new Color(_spotLight.color.r, _spotLight.color.g, _spotLight.color.b, 0.1f);
            //_fovRenderer.material.SetColor("_UnlitColor", color);
            _fovRenderer.material.color = color;
        }

        private void ReApplyColor(Color colorToApply)
        {
            _spotLight.color = colorToApply;
            //Color color = new Color(colorToApply.r, colorToApply.g, colorToApply.b, 0.1f);
            //_fovRenderer.material.SetColor("_UnlitColor", color);

            ReApplyColor();
        }


        private void Update()
        {
            if (_enemyState == EnemyState.Patrol)
            {
                FieldOfViewHandler();
            }

            HandleEnemyAIStates();
        }

        private void FieldOfViewHandler()
        {
            if (CanSeePlayer())
            {
                _playerVisibleTimer += Time.deltaTime;
            }
            else
            {
                _playerVisibleTimer -= Time.deltaTime;
            }

            _playerVisibleTimer = Mathf.Clamp(_playerVisibleTimer, 0, _timeToSpotPlayer);
            _spotLight.color = Color.Lerp(_originalSpotlightColor, Color.red, _playerVisibleTimer / _timeToSpotPlayer);
            ReApplyColor();

            if (_playerVisibleTimer >= _timeToSpotPlayer)
            {
                _enemyState = EnemyState.Chase;

                ScoreManagers.Instance.IncrementTimesDetected();
                AudioSource.PlayClipAtPoint(_howlClips[UnityEngine.Random.Range(0, _howlClips.Length)], transform.position, 1f);

                if (OnGuardHasSpottedPlayer != null)
                {

                    // TODO: 
                    OnGuardHasSpottedPlayer();
                }
            }
        }

        private bool CanSeePlayer()
        {
            if (Vector3.Distance(transform.position, _player.position) < _viewDistance && !Controller.IsInvisible)
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


        #region NavMesh Movement

        private void SetWaypoints()
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = _pathHolder.GetChild(i).position;
                waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
            }
        }

        private Vector3[] GetWaypoints()
        {
            return waypoints;
        }

        void HandleEnemyAIStates()
        {
            switch (_enemyState)
            {
                case EnemyState.Patrol:
                    _navMeshAgent.speed = _patrolSpeed;
                    HandlePatrol();
                    break;
                case EnemyState.Chase:
                    _navMeshAgent.speed = _chaseSpeed;
                    HandleChase();
                    break;
                case EnemyState.Alerted:
                    _navMeshAgent.speed = _alertSpeed;
                    HandleAlertState();
                    break;
                case EnemyState.Searching:
                    _navMeshAgent.speed = _speed;
                    HandleSearch();
                    break;
                case EnemyState.Attack:
                    HandleAttack();
                    break;
            }
        }

        void HandlePatrol()
        {
            if (Vector3.Distance(transform.position, GetWaypoints()[_currentWaypointIndex]) < _patrolReachThreshold)
            {
                _currentWaypointIndex = (_currentWaypointIndex + 1) % GetWaypoints().Length;
                _navMeshAgent.SetDestination(GetWaypoints()[_currentWaypointIndex]);
            }
        }

        void HandleChase()
        {
            if (CanSeePlayer())
            {
                if (Mathf.Abs(Vector3.Distance(_player.position, transform.position)) > _attackThreshold)
                {
                    ReApplyColor(_playerSeenColor);
                    _playerLastKnownPosition = _player.position;
                    _navMeshAgent.SetDestination(_player.position);
                }
                else
                {
                    ReApplyColor(_playerSeenColor);
                    _playerLastKnownPosition = _player.position;
                    _enemyState = EnemyState.Attack;
                }
            }
            else if (!CanSeePlayer())
            {
                _enemyState = EnemyState.Alerted;
            }
        }

        void HandleAlertState()
        {
            if (!CanSeePlayer())
            {
                if (Vector3.Distance(_playerLastKnownPosition, transform.position) > _lastKnownReachThreshold)
                {
                    _navMeshAgent.SetDestination(_playerLastKnownPosition);
                }
                else
                {
                    ReApplyColor(_alertColor);
                    //Get current position then add 90 to its Y axis
                    _searchPointA = transform.eulerAngles;

                    //Get current position then substract -90 to its Y axis
                    _searchPointB = transform.eulerAngles + new Vector3(0f, 180f, 0f);

                    _enemyState = EnemyState.Searching;
                    _searchStartTimer = Time.time;
                }
            }
            else if (CanSeePlayer())
            {
                ReApplyColor(_playerSeenColor);
                _enemyState = EnemyState.Chase;
            }
        }

        void HandleSearch()
        {
            if (CanSeePlayer())
            {
                ReApplyColor(_playerSeenColor);
                _enemyState = EnemyState.Chase;
            }
            else
            {
                if (_searchStartTimer + _searchDuartion > Time.time)
                {
                    ReApplyColor(_alertColor);
                    //float rY = Mathf.SmoothStep(-45, 45, Mathf.PingPong(Time.time * _searchRotateSpeed, 1));
                    //transform.rotation = Quaternion.Euler(0, rY, 0);

                    //float phase = Mathf.Sin(_searchDuartion + Time.deltaTime / _searchDuartion);
                    //transform.localRotation = Quaternion.Euler(new Vector3(0, phase * 360, 0));

                    //float time = Mathf.PingPong(Time.deltaTime * _searchRotateSpeed, _searchDuartion);
                    //transform.eulerAngles = Vector3.Lerp(_searchPointA, _searchPointB, time);

                    var factor = Mathf.PingPong(Time.time / _searchDuartion, 1);
                    // Optionally you can even add some ease-in and -out
                    factor = Mathf.SmoothStep(0, 1, factor);

                    // Now interpolate between the two rotations on the current factor
                    transform.rotation = Quaternion.Slerp(Quaternion.Euler(_searchPointA), Quaternion.Euler(_searchPointB), factor);
                }
                else
                {
                    ReApplyColor(_alertColor);
                    _navMeshAgent.SetDestination(GetWaypoints()[_currentWaypointIndex]);
                    _enemyState = EnemyState.Patrol;
                }
            }
        }

        void HandleAttack()
        {

            if (CanSeePlayer())
            {
                if (Mathf.Abs(Vector3.Distance(_player.position, transform.position)) > _attackThreshold)
                {
                    _playerLastKnownPosition = _player.position;
                    _enemyState = EnemyState.Chase;
                }
                else
                {
                    _playerLastKnownPosition = _player.position;
                    _navMeshAgent.SetDestination(transform.position);

                    transform.LookAt(_player);

                    ShootLogic();
                }
            }
            else
            {
                _enemyState = EnemyState.Alerted;
            }

        }

        private void ShootLogic()
        {
            _lastShot -= Time.deltaTime;

            if (_lastShot > 0)
            {
                return;
            }

            _lastShot = _fireRate;

            AudioSource.PlayClipAtPoint(_attackClips[UnityEngine.Random.Range(0, _attackClips.Length)], transform.position, 1f);
            GameObject bullet = Instantiate(_bullet, _firePosition.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 750);

            //Destroy(_bullet, 1f);
        }
        #endregion

        #region Non Navmesh Movement
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
        #endregion

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