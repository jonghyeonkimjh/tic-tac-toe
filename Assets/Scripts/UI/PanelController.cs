using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PanelController : MonoBehaviour
{
    [SerializeField] private RectTransform panelRectTransform; // 팝업창
    private CanvasGroup _backgroundCanvasGroup; // 뒤에 검은 배경

    private void Awake() {
        _backgroundCanvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Panel 표시 함수
    /// </summary>
    public void Show()
    {
        _backgroundCanvasGroup.alpha = 0;
        panelRectTransform.localScale = Vector3.zero;

        _backgroundCanvasGroup.DOFade(1, 0.3f).SetEase(Ease.Linear);
        panelRectTransform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// Panel 숨기기 함수
    /// </summary>
    public void Hide(Action callback = null)
    {
        _backgroundCanvasGroup.alpha = 1;
        panelRectTransform.localScale = Vector3.one;

        var fadeAnimation = _backgroundCanvasGroup.DOFade(0f, 0.3f).SetEase(Ease.Linear);
        var scaleAnimation = panelRectTransform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        DOTween.Sequence()
            .Join(fadeAnimation)
            .Join(scaleAnimation)
            .OnComplete(() =>
            {
                callback?.Invoke();
                Destroy(gameObject);
            });
    }
}
