// GENERATED AUTOMATICALLY FROM 'Assets/_/1 - Game/Module - Input Control/Data Assets/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace JoyBrick.Walkio.Game.InputControl.Generated
{
    public class @Controls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""New action map"",
            ""id"": ""a04ec26b-6fc5-4010-913c-d2848bf71eb4"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""8f61875f-c0c8-4d2f-8e49-aa3c35caa51f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""AWSD"",
                    ""id"": ""fbf9ec4d-38e0-4e61-80ac-4ddc1a8bf224"",
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
                    ""id"": ""99fcc092-fe72-401a-a6a7-4cd319c037dd"",
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
                    ""id"": ""840d728c-3845-4d4a-84cb-80d9fdce50fd"",
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
                    ""id"": ""e8643e69-d08f-4470-b67b-0b988bf45cdc"",
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
                    ""id"": ""2d8a31ba-0cb4-493d-8518-7cfac554244a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // New action map
            m_Newactionmap = asset.FindActionMap("New action map", throwIfNotFound: true);
            m_Newactionmap_Move = m_Newactionmap.FindAction("Move", throwIfNotFound: true);
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

        // New action map
        private readonly InputActionMap m_Newactionmap;
        private INewactionmapActions m_NewactionmapActionsCallbackInterface;
        private readonly InputAction m_Newactionmap_Move;
        public struct NewactionmapActions
        {
            private @Controls m_Wrapper;
            public NewactionmapActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Newactionmap_Move;
            public InputActionMap Get() { return m_Wrapper.m_Newactionmap; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(NewactionmapActions set) { return set.Get(); }
            public void SetCallbacks(INewactionmapActions instance)
            {
                if (m_Wrapper.m_NewactionmapActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_NewactionmapActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_NewactionmapActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_NewactionmapActionsCallbackInterface.OnMove;
                }
                m_Wrapper.m_NewactionmapActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                }
            }
        }
        public NewactionmapActions @Newactionmap => new NewactionmapActions(this);
        public interface INewactionmapActions
        {
            void OnMove(InputAction.CallbackContext context);
        }
    }
}
