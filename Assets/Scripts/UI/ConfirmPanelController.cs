using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ConfirmPanelController : PanelController
{
    [SerializeField] private TMP_Text messageText;


    public delegate void OnConfirmButtonClickHandler();
    private OnConfirmButtonClickHandler _onConfirmButtonClick;
    
    public void Show(string message, OnConfirmButtonClickHandler onConfirmButtonClick)
    {
        messageText.text = message;
        _onConfirmButtonClick = onConfirmButtonClick;
        base.Show();
    }

    /// <summary>
    /// Confirm 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickConfirmButton()
    {
        Hide(()=> _onConfirmButtonClick.Invoke());
    }
    
    /// <summary>
    /// X버튼 클릭시 홀출되는 함수
    /// </summary>
    public void OnClickCloseButton()
    {
        Hide();
    }
}
