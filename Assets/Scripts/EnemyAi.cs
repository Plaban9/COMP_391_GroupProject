using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Chase,
        GoingBackToPatrol
    }


    private State state;
    private Vector3 _startingPosition;
    private NavMeshAgent _navMeshAgent;

    [SerializeField]
    private Transform[] _waypoints;
    private Vector3 _target;
    private int _waypointIndex;

    [SerializeField]
    private float _distanceThreshold;

    [SerializeField]
    private float _radius;
    [SerializeField]
    private float _giveUpChaseRadius;

    [SerializeField]
    private FieldOfView _fieldOfView;

    [SerializeField]
    private MeshRenderer _fovRenderer;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _waypointIndex = 0;
        UpdateDestination();
        state = State.Patrol;

        _fieldOfView = GetComponent<FieldOfView>();

        _fovRenderer.material = Instantiate(Resources.Load("Target") as Material);
    }

    private void UpdateDestination()
    {
        if (_waypointIndex < _waypoints.Length && _waypoints.Length > 0)
        {
            _target = _waypoints[_waypointIndex].position;
            _navMeshAgent.SetDestination(_target);
        }
    }

    private void IterateWaypointIndex()
    {
        _waypointIndex++;

        if (_waypointIndex == _waypoints.Length)
        {
            _waypointIndex = 0;
        }
    }


    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            case State.Patrol:
                if (_fovRenderer != null)
                    _fovRenderer.material.color = Color.green;
                if (Vector3.Distance(transform.position, _target) <= _distanceThreshold)
                {
                    IterateWaypointIndex();
                    UpdateDestination();
                }
                FindPlayer();
                break;
            case State.Chase:
                _navMeshAgent.SetDestination(Controller.GetPlayer().transform.position);

                if (Vector3.Distance(transform.position, _target) > _giveUpChaseRadius)
                {
                    state = State.GoingBackToPatrol;
                }

                if (_fovRenderer != null)
                    _fovRenderer.material.color = Color.red;
                break;
            case State.GoingBackToPatrol:
                if (_fovRenderer != null)
                    _fovRenderer.material.color = Color.yellow;

                UpdateDestination();
                if (Vector3.Distance(transform.position, _target) <= _distanceThreshold * 2)
                {
                    state = State.Patrol;
                }
                break;
        }

    }

    private void FindPlayer()
    {
        if (Vector3.Distance(transform.position, Controller.GetPlayer().transform.position) < _radius && _fieldOfView.GetVisibleTargets().Count > 0)
        {
            state = State.Chase;
        }
    }

}
