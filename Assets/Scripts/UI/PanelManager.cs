using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private PanelController startPanelController;
    public enum PanelType 
    {
        StartPanel,WinPanel,DrawPanel,LosePanel
    }

    public void StartPanel(PanelType panelType)
    {
        switch (panelType)
        {
            case PanelType.StartPanel:
                startPanelController.Show();
                break;
            case PanelType.WinPanel:
                break;
            case PanelType.DrawPanel:
                break;
            case PanelType.LosePanel:
                break;
        }
    }
}
