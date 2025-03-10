public class Constants
{
    public const string ServerURL = "http://localhost:3000";
    public const string GameServerURL = "ws://localhost:3000";
    public enum MultplayManagerState { CreateRoom, JoinRoom, ExitRoom, StartGame, EndGame } // 방 생성, 방 참가, 방 퇴장, 게임 시작, 게임 종료 및 접속 종료
    public enum PlayerType {None, PlayerA, PlayerB, PlayerC, PlayerD }
    public enum GameType { SinglePlayer, DualPlayer, MultiPlayer }
}
