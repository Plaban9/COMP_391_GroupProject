using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.SceneManagement;

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
    public int TimesDetected { get => _timesDetected; private set => _timesDetected = value; }
    public int ChestTaken { get => _chestTaken; private set => _chestTaken = value; }
    public int Health { get => _health; private set => _health = value; }
    public int PowerUpCollected { get => _powerUpCollected; private set => _powerUpCollected = value; }

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
        TimesDetected++;
    }

    public void IncrementChestTaken()
    {
        ChestTaken++;
    }

    public void IncrementPowerUpCollected()
    {
        PowerUpCollected++;
    }

    public void SetHealthRemaining(int health)
    {
        this.Health = health;
    }

    public void ResetData()
    {
        this.Health = 0;
        this.TimesDetected = 0;
        this.ChestTaken = 0;
        this.PowerUpCollected = 0;
    }


    void OnEnable()
    {
        Debug.Log("OnEnable called");
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);

        if (scene.name.Equals("Start"))
        {
            ResetData();
        }
    }
}
