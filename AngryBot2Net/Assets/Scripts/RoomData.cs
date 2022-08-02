using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class RoomData : MonoBehaviour
{
    private RoomInfo _roomInfo;
    private TMP_Text _roomInfoText;
    private PhotonManager _photonManager;

    public RoomInfo roomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            _roomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }

    private void Awake()
    {
        _roomInfoText = GetComponentInChildren<TMP_Text>();
        _photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }

    private void OnEnterRoom(string roomName)
    {
        _photonManager.SetUserID();

        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = _roomInfo.MaxPlayers;
        ro.IsOpen = _roomInfo.IsOpen;
        ro.IsVisible = _roomInfo.IsVisible;
        PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
    }
}