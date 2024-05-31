using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private PlayerControls _playerControls;
    private Rigidbody _rigidbody;
    private Vector3 _movement;
    private bool movingInGrass;
    private float stepTimer;
    private int stepsToEncounter;
    private Vector3 scale;

    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] private int stepsInGrass;
    [SerializeField] private int speed;
    [SerializeField] private int encounterMinSteps;
    [SerializeField] private int encounterMaxSteps;

    private PartyManager partyManager;

    private const string IS_WALK_PARAM = "IsWalk";
    private const float TIME_PER_STEP = 0.5f;
    private const string BATTLE_SCENE = "BattleScene";
    
    void Awake()
    {
        _playerControls = new PlayerControls();
        CalculateStepsToNextEncounter();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();

        if (partyManager.GetPosition() != Vector3.zero)
        {
            transform.position = partyManager.GetPosition();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = _playerControls.Player.Move.ReadValue<Vector2>();
        float moveX = move.x;
        float moveZ = move.y;
        
        _movement = new Vector3(moveX, 0, moveZ).normalized;
        
        _animator.SetBool(IS_WALK_PARAM, _movement != Vector3.zero);

        if (moveX != 0 && moveX < 0)
        {
            _sprite.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        }

        if (moveX != 0 && moveX > 0)
        {
            _sprite.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
        }
    }
    
    private void FixedUpdate()
    {
        Move();
        CheckSteps();
    }
    
    private void Move()
    {
        _rigidbody.MovePosition(transform.position + (_movement * speed * Time.fixedDeltaTime));
    }

    private void CheckSteps()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f, grassLayer);
        movingInGrass = hitColliders.Length > 0 && _movement != Vector3.zero;

        if (movingInGrass)
        {
            stepTimer += Time.fixedDeltaTime;
            
            if (stepTimer >= TIME_PER_STEP)
            {
                stepTimer = 0;
                stepsInGrass++;
                
                if (stepsInGrass >= stepsToEncounter)
                {
                    stepsInGrass = 0;
                    partyManager.SetPosition(transform.position);
                    CalculateStepsToNextEncounter();
                    SceneManager.LoadScene(BATTLE_SCENE);
                }
            }
        }
    }
    
    private void OnDisable()
    {
        _playerControls.Disable();
    }

    private void CalculateStepsToNextEncounter()
    {
        stepsToEncounter = Random.Range(encounterMinSteps, encounterMaxSteps);
    }

    public void SetOverworldVisuals(Animator animator, SpriteRenderer spriteRenderer, Vector3 playerScale)
    {
        this._animator = animator;
        this._sprite = spriteRenderer;
        this.scale = playerScale;
    }
}
