using UnityEngine;
using UnityEngine.InputSystem.XInput;

/// <summary>
/// Handles input-driven attack logic for a FighterBase character.
/// </summary>
public class FighterController : MonoBehaviour
{
    private FighterBase fighter;
    // private TEMP input; | WIP AV KALLE

    private void Start()
    {
        fighter = GetComponent<FighterBase>();
        // input = GetComponent<TEMP>(); | WIP AV KALLE

        if (fighter == null)
            Debug.LogError("No FighterBase found on GameObject!");
        /*
        if (input == null)
            Debug.LogError("No IInputController found on GameObject!");
        */
    }

    private void Update()
    {
        /*
        if (input != null && input.IsAttacking())
        {
            var target = FindOpponent();
            if (target != null)
            {
                fighter.Attack(target);
            }
        }
        */
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