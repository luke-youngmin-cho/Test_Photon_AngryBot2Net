using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerInfo : MonoBehaviour
{
    private PhotonView _photonView;
    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        gameObject.tag = _photonView.Owner.NickName + _photonView.Owner.ActorNumber;
    }
}