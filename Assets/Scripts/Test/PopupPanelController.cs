using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class PopupPanelController : Singleton<PopupPanelController>
{
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text confirmButtonText;
    
    [SerializeField] private RectTransform panelRectTransform;
    private CanvasGroup _canvasGroup;
    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide(false);
    }

    public void Show(string content , string confirmButtonLabel, bool animation, Action confirmAction)
    {
        gameObject.SetActive(true);
        
        // 애니메이션을 위한 초기화
        _canvasGroup.alpha = 0;
        panelRectTransform.localScale = Vector3.zero;

        if (animation)
        {
            panelRectTransform.DOScale(1f, 0.2f);
            // 점진적으로 값을 업데이트 하는 기능
            _canvasGroup.DOFade(1f, 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            panelRectTransform.localScale = Vector3.one;
            _canvasGroup.alpha = 1f;
        }
        
        contentText.text = content;
        confirmButtonText.text = confirmButtonLabel;
        confirmButton.onClick.AddListener(() =>
        {
            confirmAction();
            Hide(true);
        });
    }

    public void Hide(bool animation)
    {
        if (animation)
        {
            panelRectTransform.DOScale(0f, 5f).OnComplete(() =>
            {
                contentText.text = "";
                confirmButtonText.text = "";
                confirmButton.onClick.RemoveAllListeners();

                gameObject.SetActive(false);
            });
            // 점진적으로 값을 업데이트 하는 기능
            _canvasGroup.DOFade(0f, 5f).SetEase(Ease.InBack);
        }
        else
        {
            contentText.text = "";
            confirmButtonText.text = "";
            confirmButton.onClick.RemoveAllListeners();

            gameObject.SetActive(false);
        }
    }
}
