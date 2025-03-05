using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public struct SignupData
{
    public string username;
    public string nickname;
    public string password;
}
public class SignupPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField confirmPasswordInputField;
    
    public void OnClickConfirmButton()
    {
        var username = usernameInputField.text;
        var nickname = nicknameInputField.text;
        var password = passwordInputField.text;
        var confirmPassword = confirmPasswordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(confirmPassword))
        {
            //TODO: 입력값이 비어있음을 알리는 팝업창 표시, early return
            GameManager.Instance.OpenConfirmPanel("입력 내용이 누락되었습니다.",
                    () =>
                    {
                        
                    });
            return;
        }


        if (password.Equals(confirmPassword))
        {
            var signupData = new SignupData();
            signupData.username = username;
            signupData.nickname = nickname;
            signupData.password = password;
            
            // TODO: 서버로 SignupData 전달하면서 회원가입 진행
            StartCoroutine(NetworkManager.Instance.Signup(signupData,
                    () =>
                    {
                        Destroy(gameObject);
                    },
                    () =>
                    {
                        usernameInputField.text = "";
                        nicknameInputField.text = "";
                        passwordInputField.text = "";
                        confirmPasswordInputField.text = "";
                    }));
            
        }
    }

    private IEnumerator Signup(SignupData signupData)
    {
        string jsonString = JsonUtility.ToJson(signupData); 
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        
        // UnityWebRequest www = new UnityWebRequest(serverURL + "/users/signup", UnityWebRequest.kHttpVerbPOST);
        // www.Dispose();

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/signup", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + www.error);
                if (www.responseCode == 409)
                {
                    //TODO: 중복 사용자 생성
                    Debug.Log("중복 사용자 생성");
                    GameManager.Instance.OpenConfirmPanel("이미 존재하는 사용자입니다.",
                            () =>
                            {
                                usernameInputField.text = "";
                                nicknameInputField.text = "";
                                passwordInputField.text = "";
                                confirmPasswordInputField.text = "";
                            });
                }
            }
            else
            {
                var result = www.downloadHandler.text;
                Debug.Log("Result:"  + result);
                // TODO: 회원가입 성공 팝업 표시
                GameManager.Instance.OpenConfirmPanel("회원 가입이 완료되었습니다.",
                        () =>
                        {
                            Destroy(gameObject);
                        });
            }
        }
        
        yield return null;
    }
    
    public void OnClickCancelButton()
    {
        Debug.Log("OnClickCancelButton");
    }
}
