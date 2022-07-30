using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

[RequireComponent(typeof(PhotonTransformView))]
public class PlayerMoveWithTransformView : MonoBehaviour
{
    private CharacterController _controller;
    private Transform _transform;
    private Animator _animator;
    private Camera _camera;

    private Plane _plane;
    private Ray _ray;
    private Vector3 _hitPoint;

    public float moveSpeed = 10.0f;

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
}
