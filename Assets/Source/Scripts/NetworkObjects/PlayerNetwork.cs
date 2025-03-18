using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float maxVelocity = 10.0f;

    [SerializeField]
    private CharacterController characterController;

    private void Update()
    {
        if (!IsOwner) return;
        var directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        characterController.Move(directionVector * (maxVelocity * Time.deltaTime));
    }
}

