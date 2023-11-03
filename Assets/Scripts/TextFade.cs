using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class TextFade : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUi;
    private Animator _animation;
    // Start is called before the first frame update

    public static TextFade Instance { get; private set; }
    void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        _textMeshProUGUi = GetComponent<TextMeshProUGUI>();
        _animation = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowFade(string text)
    {
        _textMeshProUGUi.text = text;
        _animation.SetTrigger("startFade");

    }
}
