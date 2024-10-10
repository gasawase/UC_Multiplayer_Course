using Unity.Netcode;
using UnityEngine;

namespace Resource.Scripts.Player
{

    public class PlayerMovement : NetworkBehaviour
    {
        private void Update()
        {
            if (!IsOwner) return; // if you are not the owner of this object, end this function
            Vector3 moveDirection = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
            if (Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
            if (Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
            if (Input.GetKey(KeyCode.D)) moveDirection.x = +1f;

            float moveSpeed = 3f;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        }
    }
}
