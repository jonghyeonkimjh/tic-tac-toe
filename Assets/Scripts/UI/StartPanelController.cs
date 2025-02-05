using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanelController : PanelController
{
    public void OnClickSinglePlayButton()
    {
        GameManager.Instance.StartGame();
        Hide();
    }
    
    public void OnClickDualPlayButton()
    {
    }
    
    public void OnClickSettingsPlayButton()
    {
    }
}
