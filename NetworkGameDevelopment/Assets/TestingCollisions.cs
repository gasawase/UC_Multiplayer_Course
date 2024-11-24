using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingCollisions : MonoBehaviour
{
    [SerializeField] private Collider colliderObject;
    void FixedUpdate()
    {
        CheckCollisions();
    }

    void CheckCollisions()
    {
        if (colliderObject == null)
        {
            Debug.LogWarning("Target Collider is not assigned.");
            return;
        }

        // Create an array to store colliders that overlap with the targetCollider
        Collider[] overlappingColliders = Physics.OverlapBox(
            colliderObject.bounds.center,
            colliderObject.bounds.extents,
            colliderObject.transform.rotation);

        // Check for overlaps
        if (overlappingColliders.Length > 0)
        {
            foreach (var collider in overlappingColliders)
            {
                if (collider != colliderObject) // Exclude the targetCollider itself
                {
                    Debug.Log($"Overlapping with: {collider.gameObject.name}");
                }
            }
        }
        else
        {
            Debug.Log("No overlaps detected.");
        }
    }
}
