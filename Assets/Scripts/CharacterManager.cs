using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject joinPopup;
    [SerializeField] private TextMeshProUGUI joinPopupText;
    
    private bool infrontOfPartyMember;

    private GameObject joinableMember;
    private PlayerControls playerControls;

    private const string NPC_JOINABLE_TAG = "NPCJoinable";
    private const string PARTY_JOINED_MESSAGE = " joined the party!";
    
    
    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        playerControls.Player.Interact.performed += _ => Interact();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }
    
    private void OnDisable()
    {
        playerControls.Disable();
    }
    
    void Update()
    {
        
    }

    private void Interact()
    {
        if (infrontOfPartyMember && joinableMember != null)
        {
            this.AddMember(joinableMember.GetComponent<JoinableCharacter>().MemberToJoin);
            infrontOfPartyMember = false;
            joinableMember = null;
        }
    }

    private void AddMember(PartyMemberInfo partyMember)
    {
        GameObject.FindFirstObjectByType<PartyManager>().AddMemberToPartyByName(partyMember.MemberName);
        var joinableCharacter = joinableMember.GetComponent<JoinableCharacter>();
        joinableCharacter.CheckIfJoined();
        joinableCharacter.ShowInteractPrompt(false);
        joinPopupText.text = partyMember.MemberName + PARTY_JOINED_MESSAGE;
        joinPopup.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == NPC_JOINABLE_TAG)
        {
            infrontOfPartyMember = true;
            joinableMember = other.gameObject;
            joinableMember.GetComponent<JoinableCharacter>().ShowInteractPrompt(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == NPC_JOINABLE_TAG)
        {
            infrontOfPartyMember = false;
            joinableMember.GetComponent<JoinableCharacter>().ShowInteractPrompt(false);
            joinableMember = null;
        }
    }
}
