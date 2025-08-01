using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public static PlayerInput PlayerInput;

    public Vector2 MoveInput { get; private set; } 
    public bool JumpJustPressed { get; private set; } 
    public bool InteractInput { get; private set; } 
    public bool TalkInput { get; private set; } 
    public bool TalkWasReleased { get; private set; } 
    public bool MenuToggle { get; private set; }

    private InputAction _moveInputAction;
    private InputAction _jumpInputAction;
    private InputAction _interactInputAction;
    private InputAction _talkInputAction;
    private InputAction _menuToggleAction;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        PlayerInput = GetComponent<PlayerInput>();

        _moveInputAction = PlayerInput.actions["Move"];
        _jumpInputAction = PlayerInput.actions["Jump"];
        _interactInputAction = PlayerInput.actions["Interact"];
        _talkInputAction = PlayerInput.actions["Speak"];
        _menuToggleAction = PlayerInput.actions["MenuToggle"];
    }
    private void Update()
    {
        MenuToggle = PlayerInput.currentActionMap.FindAction("MenuToggle").WasPressedThisFrame();

        MoveInput = _moveInputAction.ReadValue<Vector2>();
        JumpJustPressed = _jumpInputAction.WasPressedThisFrame();
        InteractInput = _interactInputAction.WasPressedThisFrame();
        TalkInput = _talkInputAction.WasPressedThisFrame();
        TalkWasReleased = _talkInputAction.WasReleasedThisFrame(); //same with interact??
    }
}
