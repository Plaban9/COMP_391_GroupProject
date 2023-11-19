using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    public const int CHEST_BONUS = 100;
    public const int POWER_UP_BONUS = 50;
    public const int HEALTH_BONUS = 10;
    public const int DETECTION_DEDUCT = 10;


    [SerializeField]
    private TextMeshProUGUI _powerUpCollectedText;

    [SerializeField]
    private TextMeshProUGUI _chestText;

    [SerializeField]
    private TextMeshProUGUI _timesDetectedText;

    [SerializeField]
    private TextMeshProUGUI _healthRemainingText;

    [SerializeField]
    private TextMeshProUGUI _totalText;

    private void Start()
    {
        int total = (POWER_UP_BONUS * ScoreManagers.Instance.PowerUpCollected) + (CHEST_BONUS * ScoreManagers.Instance.ChestTaken) + (-DETECTION_DEDUCT * ScoreManagers.Instance.TimesDetected) + (HEALTH_BONUS * ScoreManagers.Instance.Health);

        _powerUpCollectedText.text = $"{POWER_UP_BONUS} X {ScoreManagers.Instance.PowerUpCollected}";
        _chestText.text = $"{CHEST_BONUS} X {ScoreManagers.Instance.ChestTaken}";
        _timesDetectedText.text = $"-{DETECTION_DEDUCT} X {ScoreManagers.Instance.TimesDetected}";
        _healthRemainingText.text = $"{HEALTH_BONUS} X {ScoreManagers.Instance.Health}";
        _totalText.text = $"{total}";
    }
}
