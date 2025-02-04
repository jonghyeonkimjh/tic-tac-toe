using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanelController : PanelController
{
    public void OnClickStartButton()
    {
        GameManager.Instance.StartGame();
        Hide();
    }
}
