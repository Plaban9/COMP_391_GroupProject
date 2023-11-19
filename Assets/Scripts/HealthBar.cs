using Cinemachine;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image _foregroundSprite;

    [SerializeField]
    private float _reduceSpeed = 2;

    private float _target = 1;

    private Camera _camera;


    private void Start()
    {
        _camera = Camera.main;
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        _target = currentHealth / maxHealth;
    }

    private void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
        //transform.forward = _camera.transform.forward;
        _foregroundSprite.fillAmount = Mathf.MoveTowards(_foregroundSprite.fillAmount, _target, _reduceSpeed * Time.deltaTime);
        _foregroundSprite.color = Color.Lerp(Color.red, Color.green, _foregroundSprite.fillAmount);
    }

    //private void LateUpdate()
    //{
    //    // Get the direction from the HP bar to the camera
    //    Vector3 cameraDirection = _camera.transform.position - transform.position;

    //    // Cancel out any rotation on the y-axis (keep the HP bar upright)
    //    cameraDirection.y = 0f;

    //    // Rotate the HP bar to face the camera
    //    transform.rotation = Quaternion.LookRotation(cameraDirection);
    //}

    private void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    private void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    void OnCameraUpdated(CinemachineBrain brain)
    {
        if (_camera != null)
        {
            transform.forward = _camera.transform.forward;
        }
    }
}
