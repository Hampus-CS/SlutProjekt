using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Enables PlayerMove component only for the owning player (netcode-safe).
/// Attach this to the same GameObject as PlayerMove.
/// </summary>
[RequireComponent(typeof(PlayerMove))]
public class PlayerMoveNetcodeEnabler : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!IsOwner)
        {
            GetComponent<PlayerMove>().enabled = false;
        }
    }

}
