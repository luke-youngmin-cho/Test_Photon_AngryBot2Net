using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public enum PhotonViewType
{
    PunObservable,
    WithTransformView,
}
public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static readonly PhotonViewType viewType = PhotonViewType.PunObservable;
    private readonly string _version = "1.0";
    private string _userId = "Luke";
    [SerializeField] private TMP_InputField _userIDInputField;
    [SerializeField] private TMP_InputField _roomNameInputField;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = _version; // 동일 버전의 유저끼리만 서로 접속 허용하도록함.
        //PhotonNetwork.NickName = _userId;
        
        Debug.Log(PhotonNetwork.SendRate);

        // 포톤 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Start()
    {
        _userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1, 21):00}");
        _userIDInputField.text = _userId;
        PhotonNetwork.NickName = _userId;
    }

    private void SetUserID()
    {
        if (string.IsNullOrEmpty(_userIDInputField.text))
        {
            _userId = $"USER_{Random.Range(1, 21):00}";
        }
        else
        {
            _userId = _userIDInputField.text;
        }

        PlayerPrefs.SetString("USER_ID", _userId);
        PhotonNetwork.NickName = _userId;
    }

    private string SetRoomName()
    {
        if (string.IsNullOrEmpty(_roomNameInputField.text))
        {
            _roomNameInputField.text = $"ROOM_{Random.Range(1,101):000}";
        }
        return _roomNameInputField.text;
    }

    // 포톤서버 접속 후 호출되는 콜백
    // 마스터 클라이언트 (룸을 생성한 방장 유저) 와 연결되었다는 콜백임.
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    // 로비 접속 후 호출되는 콜백
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandomRoom Failed {returnCode} : {message}");
        OnMakeRoomClick();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Created Room : {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }
    }

#region UI_BUTTON_EVENT
    public void OnLoginClick()
    {
        SetUserID();
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        SetUserID();

        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }

    #endregion
}
