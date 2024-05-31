using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinableCharacter : MonoBehaviour
{
    public PartyMemberInfo MemberToJoin;
    [SerializeField] private GameObject interactPrompt;

    public void ShowInteractPrompt(bool show)
    {
        interactPrompt.SetActive(show);
    }

}
