using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Chest : MonoBehaviour
{

    private Animator _animator;
    private bool isOpen;

    [SerializeField]
    private GameObject _particleEffect;

    public bool IsOpen { get => isOpen; set => isOpen = value; }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        IsOpen = false;
    }

    public void OpenTreasureBox()
    {
        if (!IsOpen)
        {
            isOpen = true;
            _animator.SetTrigger("open");
            TextFade.Instance.ShowFade("Hidden Chest Collected");
            _particleEffect.SetActive(false);

        }
    }
}
