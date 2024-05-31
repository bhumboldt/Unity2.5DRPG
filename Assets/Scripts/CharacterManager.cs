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
    private List<GameObject> overworldCharacters = new List<GameObject>();

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
        SpawnOverworldMembers();
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
        SpawnOverworldMembers();
    }

    private void SpawnOverworldMembers()
    {
        for (int i = 0; i < overworldCharacters.Count; i++)
        {
            Destroy(overworldCharacters[i]);
        }
        
        overworldCharacters.Clear();
        
        List<PartyMember> currentParty = GameObject.FindFirstObjectByType<PartyManager>().GetCurrentParty();
        
        for (int i = 0; i < currentParty.Count; i++)
        {
            var member = currentParty[i];
            
            if (i == 0)
            {
                GameObject player = gameObject;
                var playerVisual = Instantiate(member.MemberOverworldVisualPrefab, player.transform.position, Quaternion.identity);
                
                playerVisual.transform.SetParent(player.transform);
                
                player.GetComponent<PlayerController>().SetOverworldVisuals(playerVisual.GetComponent<Animator>(), playerVisual.GetComponent<SpriteRenderer>());
                overworldCharacters.Add(playerVisual);
            }
            else
            {
                Vector3 positionToSpawn = transform.position;
                positionToSpawn.x -= 1;
                
                GameObject tempMember = Instantiate(currentParty[i].MemberOverworldVisualPrefab, positionToSpawn, Quaternion.identity);
                overworldCharacters.Add(tempMember);
            }
        }
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
