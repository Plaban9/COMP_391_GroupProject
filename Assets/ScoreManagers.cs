using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class ScoreManagers : MonoBehaviour
{
    [SerializeField]
    private int _timesDetected;

    [SerializeField]
    private int _chestTaken;

    [SerializeField]
    private int _health;

    [SerializeField]
    private int _powerUpCollected;

    private static ScoreManagers instance;

    public static ScoreManagers Instance { get => instance; private set => instance = value; }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void IncrementTimesDetected()
    {
        _timesDetected++;
    }

    public void IncrementChestTaken()
    {
        _chestTaken++;
    }

    public void IncrementPowerUpCollected()
    {
        _powerUpCollected++;
    }

    public void SetHealthRemaining(int health)
    {
        this._health = health;
    }

    public void ResetData()
    {
        this._health = 0;
        this._timesDetected = 0;
        this._chestTaken = 0;
        this._powerUpCollected = 0; 
    }
}
