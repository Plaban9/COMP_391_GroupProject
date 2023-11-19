using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Chest : MonoBehaviour
{

    private Animator _animator;
    private bool isOpen;

    [SerializeField]
    private GameObject _particleEffect;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        isOpen = false;
    }

    public void OpenTreasureBox()
    {
        if (!isOpen)
        {
            _animator.SetTrigger("open");
            TextFade.Instance.ShowFade("Hidden Chest Collected");
            _particleEffect.SetActive(false);

        }
    }
}
