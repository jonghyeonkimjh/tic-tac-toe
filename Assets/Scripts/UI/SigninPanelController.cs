using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public struct SigninData
{
    public string username;
    public string password;
}

public struct SigninResult
{
    public int result;
}

public struct UserInfo
{
    public string id;
    public string username;
    public string nickname;
}

[Serializable]
public struct ScoreResult
{
    public string id;
    public string username;
    public string nickname;
    public int score;
}

public struct RankResult {
    public ScoreResult[] ranking;
}

public struct NewScoreData
{
    public int score;
}

public class SigninPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    public void OnClickSigninButton()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            //TODO: 누락된 값 입력 요청 팝업 표시
            return;
        }

        var signinData = new SigninData()
        {
                username = username,
                password = password
        };
        StartCoroutine(NetworkManager.Instance.Signin(signinData,
                () =>
                {
                    Destroy(gameObject);
                },
                (result) =>
                {
                    if (result == 0)
                    {
                        usernameInputField.text = "";
                        passwordInputField.text = "";
                    } 
                    else if (result == 1)
                    {
                        passwordInputField.text = "";
                    }
                }));
    }

    public void OnClickSignupButton()
    {
        GameManager.Instance.OpenSignupPanel();
    }

    private IEnumerator Signin(SigninData signinData)
    {
        string jsonString = JsonUtility.ToJson(signinData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);


        using (UnityWebRequest www = 
               new UnityWebRequest(Constants.ServerURL + "/users/signin", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                
                Debug.Log("Error: " + www.error);
                if (www.responseCode == 400)
                {
                    //TODO: 잘못된 요청
                    Debug.Log("필수 요소가 누락되었습니다");
                }
            }
            else
            {
                var resultString = www.downloadHandler.text;
                var result = JsonUtility.FromJson<SigninResult>(resultString);
                if (result.result == 0)
                {
                    // 유저명 유효하지 않음
                    GameManager.Instance.OpenConfirmPanel("유저네임이 유효하지 않습니다.",
                            () =>
                            {
                                    
                            });
                }
                else if (result.result == 1)
                {
                    // 패스워드가 유요하지 않음
                    GameManager.Instance.OpenConfirmPanel("패스워드가 유효하지 않습니다.",
                            () =>
                            {
                                passwordInputField.text = "";
                            });
                }
                else if (result.result == 2)
                {
                    // 성공
                    GameManager.Instance.OpenConfirmPanel("로그인에 성공하였습니다.",
                            () =>
                            {
                                Destroy(gameObject);
                            });
                }
            }
        }
    }
    
}
