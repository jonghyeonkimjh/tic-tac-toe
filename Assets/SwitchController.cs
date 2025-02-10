using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SwitchController : MonoBehaviour
{
    [SerializeField] private Image handleImage;
    private RectTransform _handleRectTransform;
    private bool _isOn;

    private void Start()
    {
        SetOn(false);
    }
    private void Awake()
    {
        _handleRectTransform = handleImage.GetComponent<RectTransform>();
    }

    private void SetOn(bool isOn)
    {
        if (isOn)
        {
            _handleRectTransform.DOAnchorPosX(14, 0.2f);
        }
        else
        {
            _handleRectTransform.DOAnchorPosX(-14, 0.2f);
        }
        _isOn = isOn;
    }

    public void OnClickSwitch()
    {
        SetOn(!_isOn);
    }
}
