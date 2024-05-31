using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberFollowAI : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private int speed;

    private float followDistance;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private Vector3 scale;
    
    private const string IS_WALK_PARAM = "IsWalk";
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        followTarget = GameObject.FindFirstObjectByType<PlayerController>().transform;
    }

    private void Awake()
    {
        scale = transform.localScale;
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, followTarget.position) >= followDistance)
        {
            _animator.SetBool(IS_WALK_PARAM, true);
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, followTarget.position, step);
            if (followTarget.position.x - transform.position.x < 0)
            {
                _spriteRenderer.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            }
            else
            {
                _spriteRenderer.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
            }
        }
        else
        {
            _animator.SetBool(IS_WALK_PARAM, false);
        }
    }
    
    public void SetFollowDistance(float distance)
    {
        followDistance = distance;
    }
}
