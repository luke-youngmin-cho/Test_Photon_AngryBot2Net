using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Player = Photon.Realtime.Player;

[RequireComponent(typeof(PhotonView))]
public class Damage : MonoBehaviour
{
    private Renderer[] _renderers;
    private int _hp;
    public int hp
    {
        get { return _hp; }
        set { _hp = value; }
    }
    public int maxHp = 100;

    private Animator _animator;
    private CharacterController _charactorController;
    private PhotonView _photonView;
    private readonly int _hashDie = Animator.StringToHash("Die");
    private readonly int _hashRespawn = Animator.StringToHash("Respawn");
    private GameManager _gameManager;
    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _animator = GetComponent<Animator>();
        _charactorController = GetComponent<CharacterController>();
        _photonView = GetComponent<PhotonView>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        hp = maxHp;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hp > 0 && 
            collision.collider.CompareTag("BULLET") &&
            collision.collider.TryGetComponent(out Bullet bullet))
        {
            if (_photonView.Owner.ActorNumber != bullet.actorNumber)
            {
                hp -= 20;
                if (_hp <= 0)
                {
                    if (_photonView.IsMine)
                    {
                        Player attacker = PhotonNetwork.CurrentRoom.GetPlayer(bullet.actorNumber);
                        string msg = string.Format("\n<color=#00ff00>{0}</color> is killed by <color=#ff0000>{1}</color>",
                                                   _photonView.Owner.NickName,
                                                   attacker.NickName);
                        _photonView.RPC("KillMessage", RpcTarget.AllBufferedViaServer, msg);
                    }
                    StartCoroutine(E_PlayerDie());
                }
            }
        }
    }

    IEnumerator E_PlayerDie()
    {
        _charactorController.enabled = false;
        _animator.SetBool(_hashRespawn, false);
        _animator.SetTrigger(_hashDie);
        yield return new WaitForSeconds(3.0f);

        _animator.SetBool(_hashRespawn, true);
        SetPlayerVisible(false);
        yield return new WaitForSeconds(1.5f);

        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        transform.position = points[idx].position;

        hp = maxHp;
        SetPlayerVisible(true);
        _charactorController.enabled = true;
    }

    private void SetPlayerVisible(bool isVisiable)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].enabled = isVisiable;
        }
    }

    [PunRPC]
    private void KillMessage(string msg)
    {
        _gameManager.AddMessage(msg);
    }
}
