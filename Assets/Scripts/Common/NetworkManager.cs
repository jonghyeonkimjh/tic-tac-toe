using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Common
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
        }
        
        public IEnumerator Signin(SigninData signinData, Action success, Action<int> failure)
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
                    var cookie = www.GetResponseHeader("Set-Cookie");
                    Debug.Log(cookie);
                    if (!string.IsNullOrEmpty(cookie))
                    {
                        int lastIndex = cookie.LastIndexOf(";");
                        string sid = cookie.Substring(0, lastIndex);
                        PlayerPrefs.SetString("sid", sid); 
                    }
                    
                    var resultString = www.downloadHandler.text;
                    var result = JsonUtility.FromJson<SigninResult>(resultString);
                    
                    if (result.result == 0)
                    {
                        // 유저명 유효하지 않음
                        GameManager.Instance.OpenConfirmPanel("유저네임이 유효하지 않습니다.",
                                () =>
                                {
                                        failure?.Invoke(0);
                                });
                    }
                    else if (result.result == 1)
                    {
                        // 패스워드가 유요하지 않음
                        GameManager.Instance.OpenConfirmPanel("패스워드가 유효하지 않습니다.",
                                () =>
                                {
                                    failure?.Invoke(1);
                                });
                    }
                    else if (result.result == 2)
                    {
                        // 성공
                        GameManager.Instance.OpenConfirmPanel("로그인에 성공하였습니다.",
                                () =>
                                {
                                    success?.Invoke();
                                });
                    }
                }
            }
        }
        public IEnumerator Signup(SignupData signupData, Action success, Action failure)
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
                                        failure?.Invoke();
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
                                    success?.Invoke();
                                });
                    }
                }
                
                yield return null;
            }

        public void GetScore()
        {
            StartCoroutine(GetScore(
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

        
        public IEnumerator GetScore(Action<ScoreResult> success, Action failure)
        {
            using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/score", UnityWebRequest.kHttpVerbGET))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                
                string sid = PlayerPrefs.GetString("sid", "");
                if (!string.IsNullOrEmpty(sid))
                {
                    www.SetRequestHeader("Cookie", sid);
                }
                
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    if (www.responseCode == 403)
                    {
                        Debug.Log("로그인이 필요합니다.");
                    }
                    
                    failure?.Invoke();
                }
                else
                {
                    var result = www.downloadHandler.text;
                    var userScore = JsonUtility.FromJson<ScoreResult>(result);
                    Debug.Log("Result:"  + userScore.score);
                    
                    success?.Invoke(userScore);
                }
            }
            yield return null;
        }

        public IEnumerator GetRanking(Action<RankResult> success, Action failure)
        {
            using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/ranking", UnityWebRequest.kHttpVerbGET))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                
                string sid = PlayerPrefs.GetString("sid", "");
                if (!string.IsNullOrEmpty(sid))
                {
                    www.SetRequestHeader("Cookie", sid);
                }
                
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    if (www.responseCode == 403)
                    {
                        Debug.Log("로그인이 필요합니다.");
                    }
                    
                    failure?.Invoke();
                }
                else
                {
                    var result = www.downloadHandler.text;
                    var userScore = JsonUtility.FromJson<RankResult>(result);
                    Debug.Log("Result:"  + userScore.ranking[0].score);
                    success?.Invoke(userScore);
                }
            }
            yield return null;
        }

        public void UpdateScore(NewScoreData scoreData)
        {
            StartCoroutine(UpdateScore(
                    scoreData,
                    (userInfo) =>
                    {
                        Debug.Log(userInfo);
                    },
                    () =>
                    {
                        // 로그인 화면 띄우기
                        Debug.Log("GetRanking fail");
                    }));
        }
        public IEnumerator UpdateScore(NewScoreData scoreData, Action<NewScoreData> success, Action failure)
        {
            string jsonString = JsonUtility.ToJson(scoreData);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
            
            using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/score", UnityWebRequest.kHttpVerbPOST))
            {
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                string sid = PlayerPrefs.GetString("sid", "");
                if (!string.IsNullOrEmpty(sid))
                {
                    www.SetRequestHeader("Cookie", sid);
                }
                
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    if (www.responseCode == 403)
                    {
                        Debug.Log("로그인이 필요합니다.");
                    }
                    
                    failure?.Invoke();
                }
                else
                {
                    var result = www.downloadHandler.text;
                    var userScore = JsonUtility.FromJson<NewScoreData>(result);
                    Debug.Log("Result:"  + userScore.score);
                    success?.Invoke(userScore);
                }
            }
            yield return null;
        }


        public void GetUser()
        {
            StartCoroutine(
                    GetUser(
                            (userInfo) =>
                            {
                                Debug.Log(userInfo);
                                PlayerPrefs.SetString("currentUserId", userInfo.id);
                                PlayerPrefs.SetString("currentUsername", userInfo.username);

                            },
                            () =>
                            {
                                // 로그인 화면 띄우기
                                Debug.Log("GetRanking fail");
                            }
                            )
                    );
        }
        public IEnumerator GetUser(Action<UserInfo> success, Action failure)
        {
            using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/user", UnityWebRequest.kHttpVerbGET))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                
                string sid = PlayerPrefs.GetString("sid", "");
                if (!string.IsNullOrEmpty(sid))
                {
                    www.SetRequestHeader("Cookie", sid);
                }
                
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    if (www.responseCode == 403)
                    {
                        Debug.Log("로그인이 필요합니다.");
                    }
                    
                    failure?.Invoke();
                }
                else
                {
                    var result = www.downloadHandler.text;
                    var userScore = JsonUtility.FromJson<UserInfo>(result);
                    Debug.Log("Result:"  + userScore.nickname);
                    
                    success?.Invoke(userScore);
                }
            }
            yield return null;
        }
    }
}
