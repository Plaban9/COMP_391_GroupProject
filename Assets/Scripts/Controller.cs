using System.Collections;
using System.Collections.Generic;

using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = _viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _viewCamera.transform.position.y)); // Last parameter for perspective Camera
        transform.LookAt(mousePos + Vector3.up * transform.position.y); // After +, so that the Character doesn't look down when rotating
        _velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * _movementSpeed;
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
    }
}
