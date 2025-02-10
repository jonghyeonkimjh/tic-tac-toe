using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
[RequireComponent(typeof(AudioSource))]
public class SwitchController : MonoBehaviour
{
    [SerializeField] private Image handleImage;
    [SerializeField] private AudioClip clickSound;
    
    public delegate void OnSwitchChangedHandler(bool isOn);
    public OnSwitchChangedHandler OnSwitchChanged;
    
    private static readonly Color32 OnColor = new Color32(242, 68, 149, 255);
    private static  readonly Color32 OffColor = new Color32(70, 93, 117, 255);
    
    private RectTransform _handleRectTransform;
    private Image _backgroundImage;
    private bool _isOn;
    private AudioSource _audioSource;
    
    private void Start()
    {
        _handleRectTransform.anchoredPosition = new Vector2(-14, 0);
        _backgroundImage.color = OffColor;
        _isOn = false;
        SetOn(false);
    }
    private void Awake()
    {
        _handleRectTransform = handleImage.GetComponent<RectTransform>();
        _backgroundImage = GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void SetOn(bool isOn)
    {
        if (isOn)
        {
            _handleRectTransform.DOAnchorPosX(14, 0.2f);
            _backgroundImage.DOBlendableColor(OnColor, 0.2f);
        }
        else
        {
            _handleRectTransform.DOAnchorPosX(-14, 0.2f);
            _backgroundImage.DOBlendableColor(OffColor, 0.2f);
        }
        OnSwitchChanged?.Invoke(isOn);
        _isOn = isOn;
    }

    public void OnClickSwitch()
    {
        if (clickSound != null) _audioSource.PlayOneShot(clickSound);
        SetOn(!_isOn);
        
    }
}
