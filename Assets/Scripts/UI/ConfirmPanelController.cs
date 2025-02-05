using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPanelController : PanelController
{
    /// <summary>
    /// Confirm 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickConfirmButton()
    {
        GameManager.Instance.InitGame();
        Hide();
    }

    
    /// <summary>
    /// X버튼 클릭시 홀출되는 함수
    /// </summary>
    public void OnClickCloseButton()
    {
        Hide();
    }
}
