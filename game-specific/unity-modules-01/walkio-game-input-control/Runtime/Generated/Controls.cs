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
            ""name"": ""Gameplay"",
            ""id"": ""bcb45c5a-1d91-4fe8-9582-ec1c07d29dc0"",
            ""actions"": [
                {
                    ""name"": ""Horizontal"",
                    ""type"": ""Value"",
                    ""id"": ""0011026e-19b3-45ed-a950-80dca4608490"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Vertical"",
                    ""type"": ""Value"",
                    ""id"": ""0f9ffe63-cbc1-4809-8ef4-fc01cea5c5d7"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""LR"",
                    ""id"": ""0d79701b-85a1-438b-ad58-283331016e19"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""2f5027e1-ccf3-4a7c-b926-578bb312eecc"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""ba109c96-e0e9-438d-a904-6461e7703d10"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""UD"",
                    ""id"": ""c71d4032-78a4-4b21-8580-14e97399e9e4"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""408a6f67-3f33-4ecb-a934-de33f5cfa65b"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""7c05eaf5-e030-4d51-a6bd-fa9680a99b95"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
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
            // Gameplay
            m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
            m_Gameplay_Horizontal = m_Gameplay.FindAction("Horizontal", throwIfNotFound: true);
            m_Gameplay_Vertical = m_Gameplay.FindAction("Vertical", throwIfNotFound: true);
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

        // Gameplay
        private readonly InputActionMap m_Gameplay;
        private IGameplayActions m_GameplayActionsCallbackInterface;
        private readonly InputAction m_Gameplay_Horizontal;
        private readonly InputAction m_Gameplay_Vertical;
        public struct GameplayActions
        {
            private @Controls m_Wrapper;
            public GameplayActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Horizontal => m_Wrapper.m_Gameplay_Horizontal;
            public InputAction @Vertical => m_Wrapper.m_Gameplay_Vertical;
            public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
            public void SetCallbacks(IGameplayActions instance)
            {
                if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
                {
                    @Horizontal.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHorizontal;
                    @Horizontal.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHorizontal;
                    @Horizontal.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHorizontal;
                    @Vertical.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnVertical;
                    @Vertical.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnVertical;
                    @Vertical.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnVertical;
                }
                m_Wrapper.m_GameplayActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Horizontal.started += instance.OnHorizontal;
                    @Horizontal.performed += instance.OnHorizontal;
                    @Horizontal.canceled += instance.OnHorizontal;
                    @Vertical.started += instance.OnVertical;
                    @Vertical.performed += instance.OnVertical;
                    @Vertical.canceled += instance.OnVertical;
                }
            }
        }
        public GameplayActions @Gameplay => new GameplayActions(this);

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
        public interface IGameplayActions
        {
            void OnHorizontal(InputAction.CallbackContext context);
            void OnVertical(InputAction.CallbackContext context);
        }
        public interface INewactionmapActions
        {
            void OnMove(InputAction.CallbackContext context);
        }
    }
}
