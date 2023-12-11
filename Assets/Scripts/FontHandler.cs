using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class FontHandler : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _textMeshUiPro;

    [SerializeField]
    TMP_FontAsset _beforeFontAsset;

    [SerializeField]
    TMP_FontAsset _afterFontAsset;

    public void OnAnimationStart()
    {
        _textMeshUiPro.font = _beforeFontAsset;
    }

    public void OnFontChangeRequest()
    {
        _textMeshUiPro.font = _afterFontAsset;
    }
}
