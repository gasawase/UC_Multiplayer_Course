using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class ServerPlayerMovement : NetworkBehaviour
{
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private NetworkAnimator _myNetAnimator;
    [SerializeField] private float _pSpeed;
    [SerializeField] private Transform _pTransform;

    public CharacterController _cc;
    public Rigidbody _rb;
    private MyPlayerInputActions _playerInput;
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return;

        // Read our player input from our input system
        Vector2 moveInput = _playerInput.Player.Movement.ReadValue<Vector2>(); // getting left, right, up, down

        bool isJumping = _playerInput.Player.Jump.triggered;
        bool isPickup = _playerInput.Player.PickupObject.triggered;

        // Determine if we are a server or a player
        if (IsServer)
        {
            // Move if server
            Move(moveInput, isJumping, isPickup);
        }
        // Send a move request rpc to move the player
        else if (IsClient)
        {
            MoveServerRPC(moveInput, isJumping, isPickup);
        }
    }


    private void Move(Vector2 _input, bool isJumping, bool isPickup)
    {
        Vector3 _moveDirection = _input.x * _pTransform.right + _input.y * _pTransform.forward;

        // Jump animation trigger
        if (isJumping) { _myAnimator.SetTrigger("JumpTrigger"); }

        // Get object animation trigger
        if (isPickup) { _myAnimator.SetTrigger("DigPocketTrigger"); }

        _myAnimator.SetBool("IsWalking", _input.x != 0 || _input.y != 0);

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
