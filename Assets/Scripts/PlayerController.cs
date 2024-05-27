using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerControls _playerControls;
    private Rigidbody _rigidbody;
    private Vector3 _movement;

    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private int speed;

    private const string IS_WALK_PARAM = "IsWalk";
    
    void Awake()
    {
        _playerControls = new PlayerControls();
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
    }
    
    private void Move()
    {
        _rigidbody.MovePosition(transform.position + (_movement * speed * Time.fixedDeltaTime));
    }
    
    private void OnDisable()
    {
        _playerControls.Disable();
    }
}
