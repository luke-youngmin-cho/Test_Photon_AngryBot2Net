using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private readonly int _hashDie = Animator.StringToHash("Die");
    private readonly int _hashRespawn = Animator.StringToHash("Respawn");

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _animator = GetComponent<Animator>();
        _charactorController = GetComponent<CharacterController>();
        hp = maxHp;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hp > 0 && collision.collider.CompareTag("BULLET"))
        {
            hp -= 20;
            if (_hp <= 0)
            {
                StartCoroutine(E_PlayerDie());
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
}
