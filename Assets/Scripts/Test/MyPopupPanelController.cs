using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyPopupPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    public delegate void OnPopupClose();
    public event OnPopupClose Callback;

    public void Setup(string message, Action onConfirm = null)
    {
        messageText.text = message;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => {
            onConfirm?.Invoke();
            ClosePopup();
        });

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(ClosePopup);
    }

    private void ClosePopup()
    {
        Callback?.Invoke(); // 델리게이트 호출 (팝업이 닫힘)
        Destroy(gameObject); // 팝업 삭제
    }
}
