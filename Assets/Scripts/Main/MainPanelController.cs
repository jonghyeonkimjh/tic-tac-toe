using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPanelController : MonoBehaviour
{
   public void OnClickSinglePlayButton()
   {
      GameManager.Instance.ChangeToGameScene(Constants.GameType.SinglePlayer);
   }
   
   
   public void OnClickDualPlayButton()
   {
      GameManager.Instance.ChangeToGameScene(Constants.GameType.DualPlayer);
   }

   public void OnClickMultiplayButton()
   {
      GameManager.Instance.ChangeToGameScene(Constants.GameType.MultiPlayer);
   }
    
   public void OnClickSettingsPlayButton()
   {
      GameManager.Instance.OpenSettingsPanel();
   }

   public void OnClickScoreButton()
   {
      GameManager.Instance.OpenLeaderboardPanel();
   }
}
