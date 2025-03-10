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

    public class MoveData
    {
        [JsonProperty("position")] 
        public int position { get; set; } //row and column (row * column ) + column;
    }
    
    public class MessageData
    {
        [JsonProperty("nickname")]
        public string nickname { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("playerType")]
        public Constants.PlayerType playerType { get; set; }
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
        public Action<MoveData> OnOpponentMove;
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
            _socket.On("exitRoom", EndGame);
            _socket.On("startGame", StartGame);
            _socket.On("endGame", EndGame);
            _socket.On("doOpponent", DoOpponent);
            _socket.On("receiveMessage", ReceiveMessage);
            
            _socket.Connect();
        }

        // 서버로  부터 상대방의 마커 정보를 받기 위한 메서드
        private void DoOpponent(SocketIOResponse response)
        {
            var data = response.GetValue<MoveData>();
            OnOpponentMove?.Invoke(data);
        }

        public void SendPlayerMove(string roomId, int position)
        {
            _socket.Emit("doPlayer", new{ roomId, position});
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

        

        private void ExitRoom(SocketIOResponse response)
        {
            _onMultiplayStateChanged?.Invoke(Constants.MultplayManagerState.ExitRoom, null);
        }
        
        private void StartGame(SocketIOResponse response)
        {
            var data = response.GetValue<RoomData>();
            _onMultiplayStateChanged?.Invoke(Constants.MultplayManagerState.StartGame, data.roomId);
        }
        
        private void EndGame(SocketIOResponse response) 
        {
            _onMultiplayStateChanged?.Invoke(Constants.MultplayManagerState.EndGame, null);
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
        
        public void SendMessage(string roomId, Constants.PlayerType playerType, int row, int column)
        {
            _socket.Emit("sendMessage", new {roomId, playerType, row, column});
        }
        
        public void LeaveRoom(string roomId)
        {
            _socket.Emit("leaveRoom", new { roomId });
        }

        /// <summary>
        /// 소켓 연결을 종료하고, 모든 이벤트 핸들러를 해제
        /// </summary>
        public void Dispose()
        {
            if (_socket != null)
            {
                // _socket.Off("createRoom");
                // _socket.Off("joinRoom");
                // _socket.Off("startGame");
                // _socket.Off("gameEnded");
                // _socket.Off("receiveMessage");

                _socket.Disconnect();
                _socket.Dispose();
                _socket = null;
            }
        }
    }
}
