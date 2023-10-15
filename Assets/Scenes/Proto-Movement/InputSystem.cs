//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scenes/Proto-Movement/InputSystem.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputSystem: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputSystem()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputSystem"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""4ebabb5d-714c-4efd-9fcf-21160313512d"",
            ""actions"": [
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""f5af7927-5a9a-4817-bd0c-2e1a505c91d2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftHandPressed"",
                    ""type"": ""Button"",
                    ""id"": ""97139d3b-3f9a-4a9c-8f0c-d396b41a39c9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftReloadPressed"",
                    ""type"": ""Button"",
                    ""id"": ""1670cea2-331b-4f8b-bc61-b44e02faccf1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightHandPressed"",
                    ""type"": ""Button"",
                    ""id"": ""a9af0d11-08bd-47f5-914b-98c89e4190ee"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightReloadPressed"",
                    ""type"": ""Button"",
                    ""id"": ""3b7e66e0-704a-4e14-9d37-29586fad5d17"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftHandReleased"",
                    ""type"": ""Button"",
                    ""id"": ""75c64c26-b1c5-423e-94f7-5e3a3662b537"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightHandReleased"",
                    ""type"": ""Button"",
                    ""id"": ""cc1cbe28-4ef0-422f-aae9-c0fea4fa5365"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""06cd3c55-7ae0-4cbd-b73b-38fc3be2be6c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TestButton"",
                    ""type"": ""Button"",
                    ""id"": ""805475cc-7ce6-4b7e-9c36-37414d3c20a0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""ac78410f-8a3c-4c16-9528-68567c16d361"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""d19ab39c-89cc-48ee-8856-2f1be3bb9922"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftShoot"",
                    ""type"": ""Button"",
                    ""id"": ""21f9d42d-0685-491a-8d5b-914842a221ab"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightShoot"",
                    ""type"": ""Button"",
                    ""id"": ""defb06ba-d48f-407c-a6bd-bf93e3a47424"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""942c2f27-634f-4f1f-9fa3-1e9e859f7a31"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a3d3519a-1780-4079-8645-b2babe95359e"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.1,y=0.1)"",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a0764146-f768-4cd2-8302-c989c83e3291"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftHandPressed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dbb8f786-7929-4cb3-af2c-fa5ad040f6ec"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftHandPressed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ecb63b80-6f7d-466c-90e7-2a842feb04a1"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftHandReleased"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""932b8418-8f51-4e8a-bc33-b42b510a98da"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftHandReleased"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a30b8b3-6cee-42ff-8e7b-5081e64c501f"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightHandPressed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9dee4c6e-452f-4c5f-b4c7-7c6977d42f24"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightHandPressed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6c00b002-047e-40ef-8c20-eecddcfc5d93"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightHandReleased"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6845bf91-6f89-495d-8701-8325745a75b1"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightHandReleased"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23ded073-7ec6-4a6d-b061-d550c5771398"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""c832e3ad-0d77-4f2a-b040-f0aff04e3361"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2ebefb1d-6e1d-4e6c-ac32-a2b007b827fc"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1f1c8ba3-1eb0-4efd-8764-93148c209c27"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""215aa72f-1fc6-42fe-9820-bb1218e4db0f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""52f45ef7-fb25-4a07-bfe6-e607a7140728"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""532297cd-cb2b-42dc-ab8d-12d7db96c82c"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TestButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8a7088f7-2f88-4802-a20f-7501b963c465"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftReloadPressed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fa9ba0c3-86de-4a67-a911-3fb7ce0755fe"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightReloadPressed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ae8a049-7e38-4507-ad03-17bc8bf473dc"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d4ee646-ab3c-4c81-a221-4ec7189feb54"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7c6a6c6-61e4-4142-9dfa-adecbd21aad6"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6f2ab6ba-9479-4baa-8802-524ebd832719"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48407042-58ab-4e22-9404-7ce0965539b5"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e0c6c32-28d2-4055-bba3-1c909c40e038"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e80dfe1d-e56a-4b77-bb94-e988411b5db7"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b131615f-b9ba-4fbd-95d7-1acaff0ac293"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightShoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Look = m_Gameplay.FindAction("Look", throwIfNotFound: true);
        m_Gameplay_LeftHandPressed = m_Gameplay.FindAction("LeftHandPressed", throwIfNotFound: true);
        m_Gameplay_LeftReloadPressed = m_Gameplay.FindAction("LeftReloadPressed", throwIfNotFound: true);
        m_Gameplay_RightHandPressed = m_Gameplay.FindAction("RightHandPressed", throwIfNotFound: true);
        m_Gameplay_RightReloadPressed = m_Gameplay.FindAction("RightReloadPressed", throwIfNotFound: true);
        m_Gameplay_LeftHandReleased = m_Gameplay.FindAction("LeftHandReleased", throwIfNotFound: true);
        m_Gameplay_RightHandReleased = m_Gameplay.FindAction("RightHandReleased", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_TestButton = m_Gameplay.FindAction("TestButton", throwIfNotFound: true);
        m_Gameplay_Sprint = m_Gameplay.FindAction("Sprint", throwIfNotFound: true);
        m_Gameplay_Jump = m_Gameplay.FindAction("Jump", throwIfNotFound: true);
        m_Gameplay_LeftShoot = m_Gameplay.FindAction("LeftShoot", throwIfNotFound: true);
        m_Gameplay_RightShoot = m_Gameplay.FindAction("RightShoot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private List<IGameplayActions> m_GameplayActionsCallbackInterfaces = new List<IGameplayActions>();
    private readonly InputAction m_Gameplay_Look;
    private readonly InputAction m_Gameplay_LeftHandPressed;
    private readonly InputAction m_Gameplay_LeftReloadPressed;
    private readonly InputAction m_Gameplay_RightHandPressed;
    private readonly InputAction m_Gameplay_RightReloadPressed;
    private readonly InputAction m_Gameplay_LeftHandReleased;
    private readonly InputAction m_Gameplay_RightHandReleased;
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_TestButton;
    private readonly InputAction m_Gameplay_Sprint;
    private readonly InputAction m_Gameplay_Jump;
    private readonly InputAction m_Gameplay_LeftShoot;
    private readonly InputAction m_Gameplay_RightShoot;
    public struct GameplayActions
    {
        private @PlayerInputSystem m_Wrapper;
        public GameplayActions(@PlayerInputSystem wrapper) { m_Wrapper = wrapper; }
        public InputAction @Look => m_Wrapper.m_Gameplay_Look;
        public InputAction @LeftHandPressed => m_Wrapper.m_Gameplay_LeftHandPressed;
        public InputAction @LeftReloadPressed => m_Wrapper.m_Gameplay_LeftReloadPressed;
        public InputAction @RightHandPressed => m_Wrapper.m_Gameplay_RightHandPressed;
        public InputAction @RightReloadPressed => m_Wrapper.m_Gameplay_RightReloadPressed;
        public InputAction @LeftHandReleased => m_Wrapper.m_Gameplay_LeftHandReleased;
        public InputAction @RightHandReleased => m_Wrapper.m_Gameplay_RightHandReleased;
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @TestButton => m_Wrapper.m_Gameplay_TestButton;
        public InputAction @Sprint => m_Wrapper.m_Gameplay_Sprint;
        public InputAction @Jump => m_Wrapper.m_Gameplay_Jump;
        public InputAction @LeftShoot => m_Wrapper.m_Gameplay_LeftShoot;
        public InputAction @RightShoot => m_Wrapper.m_Gameplay_RightShoot;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void AddCallbacks(IGameplayActions instance)
        {
            if (instance == null || m_Wrapper.m_GameplayActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Add(instance);
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
            @LeftHandPressed.started += instance.OnLeftHandPressed;
            @LeftHandPressed.performed += instance.OnLeftHandPressed;
            @LeftHandPressed.canceled += instance.OnLeftHandPressed;
            @LeftReloadPressed.started += instance.OnLeftReloadPressed;
            @LeftReloadPressed.performed += instance.OnLeftReloadPressed;
            @LeftReloadPressed.canceled += instance.OnLeftReloadPressed;
            @RightHandPressed.started += instance.OnRightHandPressed;
            @RightHandPressed.performed += instance.OnRightHandPressed;
            @RightHandPressed.canceled += instance.OnRightHandPressed;
            @RightReloadPressed.started += instance.OnRightReloadPressed;
            @RightReloadPressed.performed += instance.OnRightReloadPressed;
            @RightReloadPressed.canceled += instance.OnRightReloadPressed;
            @LeftHandReleased.started += instance.OnLeftHandReleased;
            @LeftHandReleased.performed += instance.OnLeftHandReleased;
            @LeftHandReleased.canceled += instance.OnLeftHandReleased;
            @RightHandReleased.started += instance.OnRightHandReleased;
            @RightHandReleased.performed += instance.OnRightHandReleased;
            @RightHandReleased.canceled += instance.OnRightHandReleased;
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @TestButton.started += instance.OnTestButton;
            @TestButton.performed += instance.OnTestButton;
            @TestButton.canceled += instance.OnTestButton;
            @Sprint.started += instance.OnSprint;
            @Sprint.performed += instance.OnSprint;
            @Sprint.canceled += instance.OnSprint;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @LeftShoot.started += instance.OnLeftShoot;
            @LeftShoot.performed += instance.OnLeftShoot;
            @LeftShoot.canceled += instance.OnLeftShoot;
            @RightShoot.started += instance.OnRightShoot;
            @RightShoot.performed += instance.OnRightShoot;
            @RightShoot.canceled += instance.OnRightShoot;
        }

        private void UnregisterCallbacks(IGameplayActions instance)
        {
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
            @LeftHandPressed.started -= instance.OnLeftHandPressed;
            @LeftHandPressed.performed -= instance.OnLeftHandPressed;
            @LeftHandPressed.canceled -= instance.OnLeftHandPressed;
            @LeftReloadPressed.started -= instance.OnLeftReloadPressed;
            @LeftReloadPressed.performed -= instance.OnLeftReloadPressed;
            @LeftReloadPressed.canceled -= instance.OnLeftReloadPressed;
            @RightHandPressed.started -= instance.OnRightHandPressed;
            @RightHandPressed.performed -= instance.OnRightHandPressed;
            @RightHandPressed.canceled -= instance.OnRightHandPressed;
            @RightReloadPressed.started -= instance.OnRightReloadPressed;
            @RightReloadPressed.performed -= instance.OnRightReloadPressed;
            @RightReloadPressed.canceled -= instance.OnRightReloadPressed;
            @LeftHandReleased.started -= instance.OnLeftHandReleased;
            @LeftHandReleased.performed -= instance.OnLeftHandReleased;
            @LeftHandReleased.canceled -= instance.OnLeftHandReleased;
            @RightHandReleased.started -= instance.OnRightHandReleased;
            @RightHandReleased.performed -= instance.OnRightHandReleased;
            @RightHandReleased.canceled -= instance.OnRightHandReleased;
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @TestButton.started -= instance.OnTestButton;
            @TestButton.performed -= instance.OnTestButton;
            @TestButton.canceled -= instance.OnTestButton;
            @Sprint.started -= instance.OnSprint;
            @Sprint.performed -= instance.OnSprint;
            @Sprint.canceled -= instance.OnSprint;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @LeftShoot.started -= instance.OnLeftShoot;
            @LeftShoot.performed -= instance.OnLeftShoot;
            @LeftShoot.canceled -= instance.OnLeftShoot;
            @RightShoot.started -= instance.OnRightShoot;
            @RightShoot.performed -= instance.OnRightShoot;
            @RightShoot.canceled -= instance.OnRightShoot;
        }

        public void RemoveCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameplayActions instance)
        {
            foreach (var item in m_Wrapper.m_GameplayActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnLook(InputAction.CallbackContext context);
        void OnLeftHandPressed(InputAction.CallbackContext context);
        void OnLeftReloadPressed(InputAction.CallbackContext context);
        void OnRightHandPressed(InputAction.CallbackContext context);
        void OnRightReloadPressed(InputAction.CallbackContext context);
        void OnLeftHandReleased(InputAction.CallbackContext context);
        void OnRightHandReleased(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnTestButton(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnLeftShoot(InputAction.CallbackContext context);
        void OnRightShoot(InputAction.CallbackContext context);
    }
}
