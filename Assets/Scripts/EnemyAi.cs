using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    private Vector3 _startingPosition;
    private NavMeshAgent _navMeshAgent;

    [SerializeField]
    private Transform[] _waypoints;
    private Vector3 _target;
    private int _waypointIndex;

    [SerializeField]
    private float _distanceThreshold;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _waypointIndex = 0;
        UpdateDestination();
    }

    private void UpdateDestination()
    {
        _target = _waypoints[_waypointIndex].position;
        _navMeshAgent.SetDestination(_target);
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
        if (Vector3.Distance(transform.position, _target) <= _distanceThreshold)
        {
            IterateWaypointIndex();
            UpdateDestination();
        }
    }   
}
