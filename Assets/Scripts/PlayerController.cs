using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerControls _playerControls;
    
    void Awake()
    {
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }
    
    private void OnDisable()
    {
        _playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = _playerControls.Player.Move.ReadValue<Vector2>();
        float moveX = move.x;
        float moveZ = move.y;
        
        Debug.Log(moveX + "," + moveZ);
    }
}
