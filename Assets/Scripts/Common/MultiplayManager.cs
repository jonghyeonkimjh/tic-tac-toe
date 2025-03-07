using System;
using Newtonsoft.Json;
using SocketIOClient;
using UnityEngine;

namespace Common
{
    public class RoomData
    {
        [JsonProperty("roomId")]
        public string roomId { get; set; }
    }
    
    public class UserData
    {
        [JsonProperty("userId")]
        public string userId { get; set; }
    }
    
    public class MessageData
    {
        [JsonProperty("nickname")]
        public string nickname { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("playerType")]
        public GameManager.PlayerType playerType { get; set; }
        [JsonProperty("row")]
        public int row { get; set; }
        [JsonProperty("column")]
        public int column { get; set; }
    }
    
    public class MultiplayManager: IDisposable
    {
        private SocketIOUnity _socket;
        private event Action<Constants.MultplayManagerState, string> _onMultiplayStateChanged;
        public Action<MessageData> OnReceiveMessage;
        public MultiplayManager(Action<Constants.MultplayManagerState, string> onMultiplayStateChanged)
        {
            _onMultiplayStateChanged =  onMultiplayStateChanged;
            var url = new Uri(Constants.GameServerURL);
            _socket = new SocketIOUnity(url, new SocketIOOptions()
            {
                    Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            });
            
            _socket.On("createRoom", CreateRoom);
            _socket.On("joinRoom", JoinRoom);
            _socket.On("startGame", StartGame);
            _socket.On("gameEnded", GameEnded);
            _socket.On("receiveMessage", ReceiveMessage);
            
            _socket.Connect();
        }

        private void CreateRoom(SocketIOResponse response)
        {
            var data = response.GetValue<RoomData>();
            _onMultiplayStateChanged?.Invoke(Constants.MultplayManagerState.CreateRoom, data.roomId);
        }
        
        private void JoinRoom(SocketIOResponse response)
        {
            var data = response.GetValue<RoomData>();
            _onMultiplayStateChanged?.Invoke(Constants.MultplayManagerState.JoinRoom, data.roomId);
        }
        
        private void StartGame(SocketIOResponse response)
        {
            var data = response.GetValue<UserData>();
            _onMultiplayStateChanged?.Invoke(Constants.MultplayManagerState.StartGame, data.userId);
        }
        
        private void GameEnded(SocketIOResponse response) 
        {
            var data = response.GetValue<UserData>();
            _onMultiplayStateChanged?.Invoke(Constants.MultplayManagerState.EndGame, data.userId);
        }
        private void ReceiveMessage(SocketIOResponse response) 
        {
            var data = response.GetValue<MessageData>();
            Debug.Log("ReceiveMessage");
            OnReceiveMessage?.Invoke(data);
        }

        public void SendMessage(string roomId, string nickname, string message)
        {
            _socket.Emit("sendMessage", new {roomId, nickname, message});
        }
        
        public void SendMessage(string roomId, GameManager.PlayerType playerType, int row, int column)
        {
            _socket.Emit("sendMessage", new {roomId, playerType, row, column});
        }
        
        /// <summary>
        /// 소켓 연결을 종료하고, 모든 이벤트 핸들러를 해제
        /// </summary>
        public void Disconnect()
        {
            if (_socket != null)
            {
                Debug.Log("MultiplayManager: Disconnecting from server...");

                // 이벤트 핸들러 해제
                _socket.Off("createRoom");
                _socket.Off("joinRoom");
                _socket.Off("startGame");
                _socket.Off("gameEnded");
                _socket.Off("receiveMessage");

                // 소켓 연결 해제
                _socket.Disconnect();
                _socket = null;
            }
        }

        public void Dispose()
        {
            if (_socket != null)
            {
                _socket.Disconnect();
                _socket.Dispose();
                _socket = null;
            }
        }
    }
}
