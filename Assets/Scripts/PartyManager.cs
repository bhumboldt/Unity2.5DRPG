using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberInfo[] allMembers;
    [SerializeField] private List<PartyMember> currentParty;
    [SerializeField] private PartyMemberInfo defaultMember;

    private void Awake()
    {
        AddMemberToPartyByName(defaultMember.MemberName);
    }

    public void AddMemberToPartyByName(string name)
    {
        for (int i = 0; i < allMembers.Length; i++)
        {
            if (allMembers[i].MemberName == name)
            {
                var member = allMembers[i];
                PartyMember newPartyMember = new PartyMember();
                newPartyMember.MemberName = member.MemberName;
                newPartyMember.Level = member.StartingLevel;
                newPartyMember.CurrentHealth = member.BaseHealth;
                newPartyMember.MaxHealth = member.BaseHealth;
                newPartyMember.Strength = member.BaseStrength;
                newPartyMember.Initiative = member.BaseInitiative;
                newPartyMember.CurrentExp = 0;
                newPartyMember.MaxExp = 100;
                newPartyMember.MemberBattleVisualPrefab = member.MemberBattleVisualPrefab;
                newPartyMember.MemberOverworldVisualPrefab = member.MemberOverworldVisualPrefab;
                currentParty.Add(newPartyMember);
            }
        }
    }
}

[System.Serializable]
public class PartyMember
{
    public string MemberName;
    public int Level;
    public int CurrentHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public int CurrentExp;
    public int MaxExp;
    public GameObject MemberBattleVisualPrefab;
    public GameObject MemberOverworldVisualPrefab;
}
