using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerAnimator : MonoBehaviour
{
    

    [SerializeField] private List<Sprite> idleAnimations;
    [SerializeField] private SpriteRenderer playerSP;
    //[SerializeField] private List<Animation> walkAnimations;
    //[SerializeField] private List<Animation> runAnimations;
    private PlayerInputActions _playerInputActions;
    private Vector2 _moveInput;

    private Vector2 moveInput 
    {
         get { return _moveInput; } 
         set
         {
            _moveInput = value;
            SetAnimation();
         }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Player.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        //_playerInputActions.Player.Movement.canceled += context => moveInput = Vector2.zero;

        //_playerInputActions.Player.Run.performed += context =>
        //_playerInputActions.Player.Run.canceled += context => 
    }

    void SetAnimation()
    {
        playerSP.sprite = idleAnimations[((int)Math.Round(GetDegrees(moveInput))+180) / 45];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }

    private float GetDegrees(Vector2 vector)
    {
        return Mathf.Atan2(-vector.y, vector.x) * 180 / Mathf.PI;
    }
}
