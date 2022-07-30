using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;


public class PlayerMovePunObservable : MonoBehaviour, IPunObservable
{
    private CharacterController _controller;
    private Transform _transform;
    private Animator _animator;
    private Camera _camera;

    private Plane _plane;
    private Ray _ray;
    private Vector3 _hitPoint;

    public float moveSpeed = 10.0f;

    private Vector3 _receivePos;
    private Quaternion _receiveRot;
    public float damping = 10.0f; // 수신 좌표로의 이동 및 회전의 댐핑

    private PhotonView _photonView;
    private CinemachineVirtualCamera _virtualCamera;
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
        _photonView = GetComponent<PhotonView>();
        _virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        if (_photonView.IsMine)
        {
            _virtualCamera.Follow = _transform;
            _virtualCamera.LookAt = _transform;
        }
        _plane = new Plane(_transform.up, transform.position);
    }

    private void Start()
    {
        
        
    }

    private void Update()
    {
        if (_photonView.IsMine)
        {
            Move();
            Turn();
        }
        else
        {
            _transform.position = Vector3.Lerp(_transform.position,
                                               _receivePos,
                                               Time.deltaTime * damping);
            _transform.rotation = Quaternion.Slerp(_transform.rotation,
                                                   _receiveRot,
                                                   Time.deltaTime * damping);
        }
    }

    private float h => Input.GetAxis("Horizontal");
    private float v => Input.GetAxis("Vertical");

    private void Move()
    {
        Vector3 cameraForward = _camera.transform.forward;
        Vector3 cameraRight = _camera.transform.right;
        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;

        Vector3 moveDir = cameraForward * v + cameraRight * h;
        moveDir.Set(moveDir.x, 0.0f, moveDir.z);

        _controller.SimpleMove(moveDir * moveSpeed);

        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        _animator.SetFloat("Forward", forward);
        _animator.SetFloat("Strafe", strafe);
    }

    private void Turn()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);

        float enter = 0.0f;
        _plane.Raycast(_ray, out enter);

        _hitPoint = _ray.GetPoint(enter);

        Vector3 lookDir = _hitPoint - _transform.position;
        lookDir.y = 0;
        _transform.localRotation = Quaternion.LookRotation(lookDir);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream 데이터 전송이면
        if (stream.IsWriting)
        {
            // 로컬 위치, 회전 송신
            stream.SendNext(_transform.position);
            stream.SendNext(_transform.rotation);
        }
        else
        {
            // 서버 클라이언트들의 플레이어 위치, 회전 수신
            _receivePos = (Vector3)stream.ReceiveNext();
            _receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
