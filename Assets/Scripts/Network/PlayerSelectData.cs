using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Stores each player's selected character and ready status.
/// </summary>
public class PlayerSelectData : NetworkBehaviour
{
    [Header("Networked Variables")]
    public NetworkVariable<int> SelectedCharacterId = new NetworkVariable<int>(-1); // -1 means not selected
    public NetworkVariable<bool> IsReady = new NetworkVariable<bool>(false);

    /// <summary>
    /// Called by local player to choose a character.
    /// </summary>
    /// <param name="characterId">ID of the selected character.</param>
    public void SelectCharacter(int characterId)
    {
        if (IsOwner)
        {
            SelectedCharacterId.Value = characterId;
        }
    }

    /// <summary>
    /// Called by local player to mark themselves as ready.
    /// </summary>
    public void SetReady()
    {
        if (IsOwner)
        {
            IsReady.Value = true;
        }
    }
}
