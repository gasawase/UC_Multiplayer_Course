using System.Collections;
using System.Collections.Generic;
using App.Resource.Scripts.Obj;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class ServerPlayerMovement : NetworkBehaviour
{
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private NetworkAnimator _myNetAnimator;
    [SerializeField] private float _pSpeed;
    [SerializeField] private Transform _pTransform;
    [SerializeField] private BulletSpawner _bulletSpawner;
    [SerializeField] private Camera _playerOwnedCamera;
    public CharacterController _cc;
    public Rigidbody _rb;
    private MyPlayerInputActions _playerInput;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    Vector3 _moveDirection = new Vector3(0, 0f, 0);
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

        moveInput = moveInput.normalized;
        
        bool isJumping = _playerInput.Player.Jump.triggered;
        bool isPickup = _playerInput.Player.PickupObject.triggered;

        // Determine if we are a server or a player
        if (IsServer)
        {
            // Move if server
            Move(moveInput, isJumping, isPickup);
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


    private void Move(Vector2 _input, bool isJumping, bool isPickup)
    {
        _moveDirection = _input.x * _pTransform.right + _input.y * _pTransform.forward;

        // Jump animation trigger
        if (isJumping) { _myAnimator.SetTrigger("JumpTrigger"); }

        // Get object animation trigger
        if (isPickup) { _myAnimator.SetTrigger("DigPocketTrigger"); }

        _myAnimator.SetBool("IsWalking", _input.x != 0 || _input.y != 0);
        
        if (_input.x == 0f && _input.y == 0f) return;

        // Apply movement using Rigidbody
        Vector3 velocity = _moveDirection * _pSpeed;
        _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z); // Preserve vertical velocity for gravity
    }

    
    // RPC Calls //
    [Rpc(SendTo.Server)]
    private void MoveServerRPC(Vector2 _input, bool isJumping, bool isPickup)
    {
        Move(_input, isJumping, isPickup);
    }
    
}
