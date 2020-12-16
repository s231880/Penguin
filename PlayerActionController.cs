// GENERATED AUTOMATICALLY FROM 'Assets/InputController/CharacterInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerActionController : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerActionController()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CharacterInput"",
    ""maps"": [
        {
            ""name"": ""Jump"",
            ""id"": ""3e7de7b7-4978-48f5-8317-b8f1494261cd"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""PassThrough"",
                    ""id"": ""ccd14168-d5bd-4f52-b9dd-8385897b8b90"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""eb6bcae0-0499-4bf4-a760-8c1e090e4373"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Jump
        m_Jump = asset.FindActionMap("Jump", throwIfNotFound: true);
        m_Jump_Jump = m_Jump.FindAction("Jump", throwIfNotFound: true);
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

    // Jump
    private readonly InputActionMap m_Jump;
    private IJumpActions m_JumpActionsCallbackInterface;
    private readonly InputAction m_Jump_Jump;
    public struct JumpActions
    {
        private @PlayerActionController m_Wrapper;
        public JumpActions(@PlayerActionController wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_Jump_Jump;
        public InputActionMap Get() { return m_Wrapper.m_Jump; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(JumpActions set) { return set.Get(); }
        public void SetCallbacks(IJumpActions instance)
        {
            if (m_Wrapper.m_JumpActionsCallbackInterface != null)
            {
                @Jump.started -= m_Wrapper.m_JumpActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_JumpActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_JumpActionsCallbackInterface.OnJump;
            }
            m_Wrapper.m_JumpActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
            }
        }
    }
    public JumpActions @Jump => new JumpActions(this);
    public interface IJumpActions
    {
        void OnJump(InputAction.CallbackContext context);
    }
}
