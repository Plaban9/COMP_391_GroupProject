using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private PowerUpType _powerUpType;
    public PowerUpType PowerUpType { get => _powerUpType; private set => _powerUpType = value; }
}
