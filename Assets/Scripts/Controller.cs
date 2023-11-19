using Cinemachine;

using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

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

    [SerializeField]
    private static Controller _controller;


    [SerializeField]
    private HealthBar _healthBar;

    [SerializeField]
    private float _maxHealth = 250;
    private float _currentHealth = 250;

    [SerializeField]
    private VolumeProfile _lowHPVolumeProfile;

    [SerializeField]
    private VolumeProfile _normalVolumeProfile;

    [SerializeField]
    private Volume _volume;

    [SerializeField]
    private Animator _animator;


    private static bool _hasDied;
    private static bool _isInvisible;

    [SerializeField]
    private float _thresholdNavMesh = 1f;
    //[SerializeField]
    //private CinemachineShake _cinemachineShake;

    [SerializeField]
    private GameObject _healingAura;

    [SerializeField]
    private GameObject _invisibleAura;

    [SerializeField]
    private GameObject _hitEffect;

    public static bool IsInvisible { get => _isInvisible; private set => _isInvisible = value; }

    public static Controller GetPlayer()
    {
        return _controller;
    }


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _viewCamera = Camera.main;
        _navMeshAgent.speed = _movementSpeed;
        _lineRenderer = GetComponent<LineRenderer>();
        _spriteRenderer.gameObject.SetActive(false);

        _controller = this;
        _currentHealth = _maxHealth;
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);

        _hasDied = false;
        //_cinemachineShake = GetComponent<CinemachineShake>();

        //vcam = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        //noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    bool onlyonce = true;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !_hasDied)
        {
            onlyonce = false;
            Ray ray = _viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                _navMeshAgent.enabled = true;
                _navMeshAgent.SetDestination(hit.point);
                _spriteRenderer.gameObject.SetActive(true);
                _spriteRenderer.transform.position = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
            }
        }

        DisplayLineDestination();

        if (_animator != null && !onlyonce)
        {
            //onlyonce = true;
            _animator.SetFloat("speed", _navMeshAgent.velocity.magnitude);
        }

        try
        {
            if (Vector3.Distance(transform.position, _navMeshAgent.destination) < _thresholdNavMesh)
            {
                _navMeshAgent.enabled = false;
            }
        }
        catch (Exception ignored)
        {

        }
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

    public void OnTeleport(Vector3 position)
    {
        _spriteRenderer.gameObject.SetActive(false);
        _navMeshAgent.enabled = false;
        this.transform.position = position;
        _navMeshAgent.enabled = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Bullet"))
        {
            OnDamageTaken();
            Destroy(other.gameObject);
        }

        if (other.transform.CompareTag("LevelComplete"))
        {
            ScoreManagers.Instance.SetHealthRemaining((int)_currentHealth);
            TextFade.Instance.ShowFade("Level Completed!!");
        }
        else if (other.transform.CompareTag("PowerUp"))
        {
            try
            {
                switch (other.GetComponent<PowerUp>().PowerUpType)
                {
                    case PowerUpType.HEALTH:
                        TextFade.Instance.ShowFade("Health Collected!!");
                        _currentHealth += 15f;
                        _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
                        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
                        ScoreManagers.Instance.IncrementPowerUpCollected();
                        StartCoroutine(PerformHealEffect(2f));
                        break;
                    case PowerUpType.INVISIBILITY:
                        TextFade.Instance.ShowFade("Invisibility Granted!!");
                        StartCoroutine(PerformInvsibility(10f));
                        ScoreManagers.Instance.IncrementPowerUpCollected();
                        break;
                }

                Destroy(other.gameObject);
            }
            catch (Exception ignored)
            {

            }
        }
    }

    private IEnumerator PerformHealEffect(float duration)
    {
        GameObject healAura = Instantiate(_healingAura, transform.position, Quaternion.identity, this.transform);
        yield return new WaitForSeconds(duration);
        Destroy(healAura);

    }

    private IEnumerator PerformInvsibility(float duration)
    {
        IsInvisible = true;
        GameObject inviAura = Instantiate(_invisibleAura, transform.position, Quaternion.identity, this.transform);
        yield return new WaitForSeconds(duration);
        Destroy(inviAura);
        IsInvisible = false;
    }

    private IEnumerator PerformHit(float duration)
    {
        GameObject hitEffect = Instantiate(_hitEffect, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(duration);
        Destroy(hitEffect);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Chest"))
        {
            try
            {
                ScoreManagers.Instance.IncrementChestTaken();
                collision.transform.GetComponent<Chest>().OpenTreasureBox(); ;
            }
            catch
            {

            }
        }
    }

    private void OnDamageTaken()
    {
        _currentHealth -= 15f;
        _currentHealth = Mathf.Max(0, _currentHealth);
        StartCoroutine(PerformHit(3f));

        CinemachineShake.Instance.ShakeCamera(1f, 0.1f);
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);

        if (_volume != null)
        {
            if (_currentHealth < 30f)
            {

                _volume.profile = _lowHPVolumeProfile;
            }
            else
            {
                _volume.profile = _normalVolumeProfile;
            }
        }

        if (_currentHealth <= 0 && !_hasDied)
        {
            _hasDied = true;
            _animator.SetTrigger("die");
        }
    }
}