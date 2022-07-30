using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        CreatePlayer();
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
}
