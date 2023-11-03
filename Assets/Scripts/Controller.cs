using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private Camera _viewCamera;

    [SerializeField]
    private float _movementSpeed = 6f;

    [SerializeField]
    private Vector3 _velocity;

    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    [SerializeField]
    private LineRenderer _lineRenderer;

    [SerializeField]
    private SpriteRenderer _spriteRenderer; 

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _viewCamera = Camera.main;
        _navMeshAgent.speed = _movementSpeed;
        _lineRenderer = GetComponent<LineRenderer>();
        _spriteRenderer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = _viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                _navMeshAgent.SetDestination(hit.point);
                _spriteRenderer.gameObject.SetActive(true);
                _spriteRenderer.transform.position = hit.point;

            }
        }

        DisplayLineDestination();

        //Vector3 mousePos = _viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _viewCamera.transform.position.y)); // Last parameter for perspective Camera
        //transform.LookAt(mousePos + Vector3.up * transform.position.y); // After +, so that the Character doesn't look down when rotating
        //transform.LookAt(new Vector3(mousePos.x, transform.position.y, mousePos.z)); // After +, so that the Character doesn't look down when rotating
        //_velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * _movementSpeed;
    }

    private void FixedUpdate()
    {
        //_rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
    }

    private void DisplayLineDestination()
    {
        _lineRenderer.positionCount = _navMeshAgent.path.corners.Length;
        _lineRenderer.SetPositions(_navMeshAgent.path.corners);
    }
}
