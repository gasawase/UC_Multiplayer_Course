using System;
using Unity.Netcode;
using UnityEngine;

namespace Resource.Scripts.Player
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

        private void FixedUpdate()
        {
            if (!IsOwner) return; // if you are not the owner of this object, end this function
            
            // local physical movement management 
            Vector3 moveDirection = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
            if (Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
            if (Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
            if (Input.GetKey(KeyCode.D)) moveDirection.x = +1f;
            
            // jump animation trigger
            if (Input.GetKey(KeyCode.Space)) _ownerNetworkAnimator.SetTrigger("JumpTrigger");
            // get object animation trigger
            if (Input.GetKey(KeyCode.R)) _ownerNetworkAnimator.SetTrigger("DigInPocketTrigger");
            
            _animator.SetBool("IsWalking", moveDirection.z != 0 || moveDirection.x != 0);

            float moveSpeed = 3f;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        }
    }
}
