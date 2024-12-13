using System.Collections;
using System.Collections.Generic;
using App.Resource.Scripts.Obj;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor.Experimental;

public class ServerPlayerMovement : NetworkBehaviour
{
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private NetworkAnimator _myNetAnimator;
    [SerializeField] private float _pSpeed;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private Transform _pTransform;
    [SerializeField] private BulletSpawner _bulletSpawner;
    [SerializeField] private Camera _playerOwnedCamera;
    public CharacterController _cc;
    public Rigidbody _rb;
    private MyPlayerInputActions _playerInput;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private Vector3 _moveDirection = Vector3.zero;
    private Vector2 _lookInput = Vector2.zero; // To store mouse input for rotation

    
    // Start is called before the first frame update
    void Start()
    {
        if (_myAnimator == null)
        {
            _myAnimator = gameObject.GetComponent<Animator>();
        }

        if (_myNetAnimator == null)
        {
            _myNetAnimator = gameObject.GetComponent<NetworkAnimator>();
        }

        if (_rb == null)
        {
            _rb = gameObject.GetComponent<Rigidbody>();
        }
        _playerInput = new MyPlayerInputActions();
        _playerInput.Enable();

        if (IsOwner)
        {
            Camera.main.gameObject.SetActive(false);
            _playerOwnedCamera.gameObject.SetActive(true);
        }
        else
        {
            Camera.main.gameObject.SetActive(true);
            _playerOwnedCamera.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return;

        // Read our player input from our input system
        Vector2 moveInput = _playerInput.Player.Movement.ReadValue<Vector2>(); // getting left, right, up, down
        _lookInput = _playerInput.Player.Look.ReadValue<Vector2>(); // Mouse movement

        moveInput = moveInput.normalized;
        Debug.Log($"vector3.right: {Vector3.right} | input.x: {moveInput.x}");
        
        bool isJumping = _playerInput.Player.Jump.triggered;
        bool isPickup = _playerInput.Player.PickupObject.triggered;

        // Determine if we are a server or a player
        if (IsServer)
        {
            // Move if server
            Move(moveInput, isJumping, isPickup);
            //MoveForward();
            RotatePlayer();
        }
        // Send a move request rpc to move the player
        else if (IsClient && !IsHost)
        {
            MoveServerRPC(moveInput, isJumping, isPickup);
        }

        if (isPickup)
        {
            _bulletSpawner.FireProjectileRpc();
        }
    }

    
    private void MoveForward()
    {
        Vector3 forwardDirection = _pTransform.forward; // Always move forward
        Vector3 velocity = forwardDirection * _pSpeed;

        // Apply movement using Rigidbody
        _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z); // Preserve Y velocity
    }

    private void Move(Vector2 _input, bool isJumping, bool isPickup)
    {
        _moveDirection = _input.x * Vector3.right + _input.y * Vector3.forward;
        //_moveDirection = _input.y * Vector3.forward;

        // Jump animation trigger
        if (isJumping) { _myAnimator.SetTrigger("JumpTrigger"); }

        // Get object animation trigger
        if (isPickup) { _myAnimator.SetTrigger("DigPocketTrigger"); }

        _myAnimator.SetBool("IsWalking", _input.x != 0 || _input.y != 0);
        
        if (_input.magnitude == 0f) return;

        // Apply movement using Rigidbody
        Vector3 velocity = _moveDirection * _pSpeed;
        _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z); // Preserve vertical velocity for gravity
        Debug.Log($"rigidbody velocity: {_rb.velocity}");
    }
    
    
    private void RotatePlayer()
    {
        if (_lookInput.magnitude > 0.1f)
        {
            float yaw = _lookInput.x * _rotationSpeed * Time.deltaTime;
            _pTransform.Rotate(0f, yaw, 0f);
        }
    }
    
    // RPC Calls //
    [Rpc(SendTo.Server)]
    private void MoveServerRPC(Vector2 _input, bool isJumping, bool isPickup)
    {
        Move(_input, isJumping, isPickup);
    }
    
}
