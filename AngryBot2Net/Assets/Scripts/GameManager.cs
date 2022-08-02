using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _msgList;
    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private TMP_Text _connectInfo;
    [SerializeField] private Button _exitButton;
    
    public void AddMessage(string msg)
    {
        _msgList.text += msg;
    }

    private void Awake()
    {
        CreatePlayer();
        SetRoomInfo();
        _exitButton.onClick.AddListener(() => OnExitClick());
    }
    private void CreatePlayer()
    {
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        // 네트워크상에 캐릭터생성
        switch (PhotonManager.viewType)
        {
            case PhotonViewType.PunObservable:
                PhotonNetwork.Instantiate("PlayerWithSerializeView/Player",
                                          points[idx].position,
                                          points[idx].rotation,
                                          0);
                break;
            case PhotonViewType.WithTransformView:
                PhotonNetwork.Instantiate("PlayerWithTransformView/Player",
                                          points[idx].position,
                                          points[idx].rotation,
                                          0);
                break;
            default:
                break;
        }
    }

    private void SetRoomInfo()
    {
        Room room = PhotonNetwork.CurrentRoom;
        _roomName.text = room.Name;
        _connectInfo.text = $"({room.PlayerCount} / {room.MaxPlayers})";
    }

    private void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    // 룸에서 나갈 떄 호출되는 함수 (LeaveRoom() 콜백)
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    // 룸에서 새로운 클라유저가 접속했을 때 호출되는 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();
        string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> is joined room";
        _msgList.text += msg;
    }

    // 룸에서 다른 클라유저가 나갈떄 호출되는 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomInfo();
        string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> is left room";
        _msgList.text += msg;
    }
}
