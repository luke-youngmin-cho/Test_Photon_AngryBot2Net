using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fire : MonoBehaviour
{
    [SerializeField] private Transform _firePos;
    [SerializeField] private GameObject _bulletPrefab;
    private ParticleSystem _muzzleFlash;
    private PhotonView _photonView;
    private bool isMouseClick => Input.GetMouseButtonDown(0);

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _muzzleFlash = _firePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (_photonView.IsMine && isMouseClick)
        {
            FireBullet();
            _photonView.RPC("FireBullet", RpcTarget.Others, null);
        }
    }

    // RPC : Remote Procedure Calls
    [PunRPC]
    private void FireBullet()
    {
        if (!_muzzleFlash.isPlaying)
            _muzzleFlash.Play(true);

        GameObject bullet = Instantiate(_bulletPrefab,
                                        _firePos.position,
                                        _firePos.rotation);
    }
}
