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

    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] private int stepsInGrass;
    [SerializeField] private int speed;
    [SerializeField] private int encounterMinSteps;
    [SerializeField] private int encounterMaxSteps;

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
            _sprite.flipX = true;
        }

        if (moveX != 0 && moveX > 0)
        {
            _sprite.flipX = false;
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
}
