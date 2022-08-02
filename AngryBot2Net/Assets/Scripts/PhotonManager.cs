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
    [SerializeField] private GameObject _roomItemPrefab;
    [SerializeField] private Transform _roomScrollContent;
    private Dictionary<string, GameObject> _rooms = new Dictionary<string, GameObject>();
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = _version; // ���� ������ ���������� ���� ���� ����ϵ�����.
        //PhotonNetwork.NickName = _userId;
        
        Debug.Log(PhotonNetwork.SendRate);

        // ���� ���� ����
        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Start()
    {
        _userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1, 21):00}");
        _userIDInputField.text = _userId;
        PhotonNetwork.NickName = _userId;
    }

    public void SetUserID()
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

    // ���漭�� ���� �� ȣ��Ǵ� �ݹ�
    // ������ Ŭ���̾�Ʈ (���� ������ ���� ����) �� ����Ǿ��ٴ� �ݹ���.
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    // �κ� ���� �� ȣ��Ǵ� �ݹ�
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandomRoom Failed {returnCode} : {message}");
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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tmpRoom = null;
        foreach (RoomInfo roomInfo in roomList)
        {
            Debug.Log($"Room = {roomInfo.Name} ({roomInfo.PlayerCount} / {roomInfo.MaxPlayers})");

            if (roomInfo.RemovedFromList)
            {
                if (_rooms.TryGetValue(roomInfo.Name, out tmpRoom))
                {
                    Destroy(tmpRoom);
                    _rooms.Remove(roomInfo.Name);
                }
            }
            else
            {
                if (_rooms.TryGetValue(roomInfo.Name, out tmpRoom) == false)
                {
                    tmpRoom = Instantiate(_roomItemPrefab, _roomScrollContent);
                    _rooms.Add(roomInfo.Name, tmpRoom);
                }
                tmpRoom.GetComponent<RoomData>().roomInfo = roomInfo;
            }
        }
    }


    #region UI_BUTTON_EVENT
    public void OnLoginClick()
    {
        SetUserID();
        //PhotonNetwork.JoinRandomRoom();
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
