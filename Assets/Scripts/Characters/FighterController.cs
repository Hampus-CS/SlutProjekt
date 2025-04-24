using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

/// <summary>
/// Handles input-driven attack logic for a FighterBase character, but only for the owning player.
/// </summary>
public class FighterController : NetworkBehaviour
{
    private FighterBase fighter;

    private void Start()
    {
        fighter = GetComponent<FighterBase>();

        if (fighter == null)
            Debug.LogError("No FighterBase found on GameObject!");

    }

    private void Update()
    {
        if (!IsOwner) return; // Only allow local input on owned object
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            var target = FindOpponent();
            if (target != null)
            {
                fighter.Attack(target);
            }
        }
        
    }

    private FighterBase FindOpponent()
    {
        FighterBase[] allFighters = Object.FindObjectsByType<FighterBase>(FindObjectsSortMode.None);
        foreach (var f in allFighters)
        {
            if (f != fighter)
                return f;
        }
        return null;
    }
}