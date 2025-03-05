using System.Collections;
using System.Collections.Generic;
using Common;
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

   public void OnClickScoreButton()
   {
      StartCoroutine(NetworkManager.Instance.GetScore(
            (userInfo) =>
            {
               Debug.Log(userInfo);
            },
            () =>
            {
               // 로그인 화면 띄우기
               GameManager.Instance.OpenSigninPanel();
            }));
   }
}
