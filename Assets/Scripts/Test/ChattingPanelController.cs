using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;

public class ChattingPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private GameObject messageTextPrefab;
    [SerializeField] private Transform messageTextParent;

    private MultiplayManager _multiplayManager;
    private string _roomId;
    
    public void OnEndEditInputField(string messageText)
    {
        var messageTextObject = Instantiate(messageTextPrefab, messageTextParent);
        messageTextObject.GetComponent<TMP_Text>().text = messageText;
        messageInputField.text = "";

        if (_roomId != null && _multiplayManager != null)
        {
            _multiplayManager.SendMessage(_roomId, "홍길동", messageText);
        }
    }

    private void Start()
    {
        messageInputField.interactable = false;
        _multiplayManager = new MultiplayManager(
                (state, id) =>
                {
                    switch (state)
                    {

                        case Constants.MultplayManagerState.CreateRoom:
                            Debug.Log("##Create Room");
                            _roomId = id;
                            break;
                        case Constants.MultplayManagerState.JoinRoom:
                            Debug.Log("##Join Room");
                            _roomId = id;
                            messageInputField.interactable = true;
                            break;
                        case Constants.MultplayManagerState.StartGame:
                            Debug.Log("##Start Game");
                            messageInputField.interactable = true;
                            break;
                        case Constants.MultplayManagerState.EndGame:
                            Debug.Log("##End Game");
                            messageInputField.interactable = true;
                            break;
                    }
                });
        _multiplayManager.OnReceiveMessage = OnReceiveMessage;
    }

    private void OnReceiveMessage(MessageData messageData)
    {
        UnityThread.executeInUpdate(
                () =>
                {
                    var messageObject = Instantiate(messageTextPrefab, messageTextParent);
                    messageObject.GetComponent<TMP_Text>().text = messageData.nickname + " : " + messageData.message;
                });
    }

    private void OnDestroy()
    {
        // my idea
        // _multiplayManager.OnReceiveMessage = null;
        // _multiplayManager.Disconnect();
    }

    private void OnApplicationQuit()
    {
        // lect idea
        _multiplayManager.Dispose();
    }
}
