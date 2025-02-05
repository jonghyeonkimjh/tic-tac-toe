using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public class PanelController : MonoBehaviour
{
    
    public bool IsShow {get; private set;}

    public delegate void OnHideHandler();
    private OnHideHandler _onHide;
    
    
    private RectTransform _rectTransform;
    
    /// <summary>
    /// 카메라 뷰포트 밖으로 숨기기 위한 위치 ( 초기 위치 )
    /// </summary>
    private Vector2 _hideAnchorPosition; 

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _hideAnchorPosition = _rectTransform.anchoredPosition;
        IsShow = false;
    }

    /// <summary>
    /// Panel 표시 함수
    /// </summary>
    public void Show(OnHideHandler onHide)
    {
        _onHide = onHide;
        _rectTransform.anchoredPosition = Vector2.zero;
        IsShow = true;
    }

    /// <summary>
    /// Panel 숨기기 함수
    /// </summary>
    public void Hide()
    {
        _rectTransform.anchoredPosition = _hideAnchorPosition;
        IsShow = false;
        _onHide?.Invoke();
    }
}
