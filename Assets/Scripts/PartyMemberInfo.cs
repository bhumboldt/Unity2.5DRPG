using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Party Member")]
public class PartyMemberInfo : ScriptableObject
{
    public string MemberName;
    public int StartingLevel;
    public int BaseHealth;
    public int BaseStrength;
    public int BaseInitiative;
    public GameObject MemberBattleVisualPrefab; // what will be displayed in battle
    public GameObject MemberOverworldVisualPrefab; // what will be displayed in the overworld
}
