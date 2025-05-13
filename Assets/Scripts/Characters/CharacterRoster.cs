using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character Roster")]
public class CharacterRoster : ScriptableObject
{
    public List<CharacterData> characters;
    public CharacterData GetById(int id) => characters.Find(c => c.id == id);
}
