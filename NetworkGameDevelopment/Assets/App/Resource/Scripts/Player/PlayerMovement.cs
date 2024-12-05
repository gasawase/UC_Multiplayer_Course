using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts.Player
{

    public class PlayerMovement : NetworkBehaviour
    {
        
        [SerializeField] private Animator _animator;
        [SerializeField] private OwnerNetworkAnimator _ownerNetworkAnimator;

        private void Awake()
        {
            if (_animator == null)
            {
                gameObject.AddComponent<Animator>();
            }

            if (_ownerNetworkAnimator == null)
            {
                gameObject.AddComponent<OwnerNetworkAnimator>();
            }
        }

        void FixedUpdate()
        {
            if (!IsOwner) return; // Ensure only the owner controls the object

            // Get the input direction
            Vector3 moveDirection = GetInputDirection();

            // Trigger animations
            HandleAnimations(moveDirection);

            // Move the player
            MovePlayer(moveDirection);
        }

        private Vector3 GetInputDirection()
        {
            float horizontal = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;
            float vertical = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;
            Vector3 direction = new Vector3(horizontal, 0f, vertical);

            // Normalize to prevent speed boost on diagonal movement
            return direction.normalized;
        }

        private void HandleAnimations(Vector3 moveDirection)
        {
            bool isWalking = moveDirection.magnitude > 0;
            _animator.SetBool("IsWalking", isWalking);

            if (Input.GetKey(KeyCode.Space))
                _ownerNetworkAnimator.SetTrigger("JumpTrigger");
    
            if (Input.GetKey(KeyCode.R))
                _ownerNetworkAnimator.SetTrigger("DigPocketTrigger");
        }

        private void MovePlayer(Vector3 moveDirection)
        {
            float moveSpeed = 3f;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        }

    }
}
