using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPanelController : MonoBehaviour
{
   public void OnClickSinglePlayButton()
   {
      GameManager.Instance.ChangeToGameScene(GameManager.GameType.SinglePlayer);
   }
   
   
   public void OnClickDualPlayButton()
   {
      GameManager.Instance.ChangeToGameScene(GameManager.GameType.DualPlayer);
   }
    
   public void OnClickSettingsPlayButton()
   {
      GameManager.Instance.OpenSettingsPanel();
   }
}
