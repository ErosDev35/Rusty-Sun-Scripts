using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public static PlayerInputs Instance {get; private set;}
    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    //Other
    public PlayerCharacterController player;
    //Input

    public Vector2 moveInput = Vector2.zero;
    public Vector2 camInput = Vector2.zero;
    public float tiltInput = 0;
    public bool sprintInput = false;
    public float scrollInput = 0;
    public bool rightClickInput = false;
    public bool crouchInput = false;
    public bool jumpInput = false;
    public bool interactInput = false;
    public bool dropInput = false;
    public bool craftInput = false;
    public bool buildInput = false;
    public bool escapeInput = false;
    public bool leftClickInput = false;
    public bool leftClickMaintainInput = false;
    public bool reloadInput = false;
    public bool checkBodyInput = false;
    public bool outsideInteractInput = false;
    public bool slot1Input = false;
    public bool slot2Input = false;
    public bool slot3Input = false;
    public bool inventoryInput = false;

    //...............................................................................

    //SmoothDamp
    public Vector2 currentInputVector;
    public Vector2 smoothInputVelocity;
    public void OnMove(InputAction.CallbackContext context)
    {
        this.moveInput = context.ReadValue<Vector2>();
    }
    public void OnCamMove(InputAction.CallbackContext context)
    {
        this.camInput = context.ReadValue<Vector2>() * ((context.action.activeControl.name == "rightStick")? 10 : 1);
    }
    public void OnTiltInput(InputAction.CallbackContext context)
    {
        this.tiltInput = context.ReadValue<float>();
    }
    public void OnSprintInput(InputAction.CallbackContext context)
    {
        this.sprintInput = context.performed;
    }
    public void OnScrollInput(InputAction.CallbackContext context)
    {
        this.scrollInput = context.ReadValue<float>();
    }
    public void OnRightClickInput(InputAction.CallbackContext context)
    {
        this.rightClickInput = context.performed;
    }
    public void OnCrouchInput(InputAction.CallbackContext context)
    {
        this.crouchInput = context.performed;
    }
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        this.jumpInput = context.performed;
    }
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.started) this.interactInput = true;
    }
    public void OnDropInput(InputAction.CallbackContext context)
    {
        if(context.started) this.dropInput = true;
    }
    public void OnCraftInput(InputAction.CallbackContext context)
    {
        if(context.started) this.craftInput = true;
    }
    public void OnBuildInput(InputAction.CallbackContext context)
    {
        if(context.started) this.buildInput = true;
    }
    public void OnEscapeInput(InputAction.CallbackContext context)
    {
        if(context.started) this.escapeInput = true;
    }
    public void OnLeftClickInput(InputAction.CallbackContext context)
    {
        if(context.started) this.leftClickInput = true;
    }
    public void OnLeftClickMaintainInput(InputAction.CallbackContext context)
    {
        this.leftClickMaintainInput = context.performed;
    }
    public void OnReloadInput(InputAction.CallbackContext context)
    {
        if(context.started) this.reloadInput = true;
    }
    public void OnCheckBodyInput(InputAction.CallbackContext context)
    {
        if(context.started) this.checkBodyInput = true;
    }
    public void OnOutsideInteractInput(InputAction.CallbackContext context)
    {
        if(context.started) this.outsideInteractInput = true;
    }
    public void OnSlot1Input(InputAction.CallbackContext context)
    {
        if(context.started) this.slot1Input = true;
    }
    public void OnSlot2Input(InputAction.CallbackContext context)
    {
        if(context.started) this.slot2Input = true;
    }
    public void OnSlot3Input(InputAction.CallbackContext context)
    {
        if(context.started) this.slot3Input = true;
    }
    public void OnInventoryInput(InputAction.CallbackContext context)
    {
        if(context.started) {
            player.BagCheck();
        }
    }
}
