using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinableCharacter : MonoBehaviour
{
    public PartyMemberInfo MemberToJoin;
    [SerializeField] private GameObject interactPrompt;

    private void Start()
    {
        CheckIfJoined();
    }

    public void ShowInteractPrompt(bool show)
    {
        interactPrompt.SetActive(show);
    }

    public void CheckIfJoined()
    {
        var party = GameObject.FindFirstObjectByType<PartyManager>().GetCurrentParty();

        foreach (var member in party)
        {
            if (member.MemberName == MemberToJoin.MemberName)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
