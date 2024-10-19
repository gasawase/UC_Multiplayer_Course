using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _pSpeed;
    [SerializeField] private Transform _pTransform;

    public CharacterController _cc;
    private MyPlayerInputActions _playerInput;
    // Start is called before the first frame update
    void Start()
    {
        _playerInput = new MyPlayerInputActions();
        _playerInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        
        // Read our player input from our input system
        Vector2 moveInput = _playerInput.Player.Movement.ReadValue<Vector2>(); // getting left, right, up, down
        
        // Determine if we are a server or a player
        if (IsServer)
        {
            // Move if server
            Move(moveInput);
        }
        // Send a move request rpc to move the player
        else if (IsClient)
        {
            MoveServerRPC(moveInput);
        }
    }

    private void Move(Vector2 _input)
    {
        Vector3 _moveDirection = _input.x * _pTransform.right+ _input.y * _pTransform.forward;
        
        _cc.Move(_moveDirection * _pSpeed * Time.deltaTime);
    }

    
    // RPC Calls //
    [Rpc(SendTo.Server)]
    private void MoveServerRPC(Vector2 _input)
    {
        Move(_input);
    }
    
}
