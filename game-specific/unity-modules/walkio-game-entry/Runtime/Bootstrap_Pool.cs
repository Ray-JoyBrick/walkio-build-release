namespace JoyBrick.Walkio.Game
{
    using BehaviorDesigner.Runtime;
    using HellTap.PoolKit;
    using Opsive.UltimateCharacterController.Camera;
    using Opsive.UltimateCharacterController.Input;
    using Opsive.UltimateCharacterController.Utility;
    using Pathfinding;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AI.Planner.Controller;
    using UnityEngine.SceneManagement;

    //
    using GameInputControl = JoyBrick.Walkio.Game.InputControl;

#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
    
    using GameBattle = JoyBrick.Walkio.Game.Battle;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    using GameStageFlowControl = JoyBrick.Walkio.Game.StageFlowControl;
    
#endif

    public partial class Bootstrap :
        IPoolKitListener
    {
        public GameObject quickPlayerCamera;
        
        private void CreateNpcTeamLeaderFromPool()
        {
            var pool = PoolKit.Find("Team Leader Pool");
            if (pool == null) return;
            
            // Should create from factory and give location later
            var randomPosition = new Vector3(
                Random.Range(0, 100.0f),
                0,
                Random.Range(0, 100.0f));
            var spawned = pool.Spawn("Character_BusinessMan_Shirt_01", randomPosition, Quaternion.identity);

            //
            var decisionController = spawned.GetComponent<DecisionController>();
            if (decisionController != null)
            {
                decisionController.enabled = true;
            }

            var behaviorTree = spawned.GetComponent<BehaviorTree>();
            if (behaviorTree != null)
            {
                behaviorTree.enabled = true;
                
                behaviorTree.EnableBehavior();
            }

            var controllingRoute = spawned.GetComponent<GameInputControl.ControllingRoute>();
            if (controllingRoute != null)
            {
                controllingRoute.autoGenerate = true;
            }

            // var aiLerp = spawned.GetComponent<AILerp>();
            // if (aiLerp != null)
            // {
            //     aiLerp.enabled = true;
            // }

            var manageUnitSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameBattle.ManageUnitSystem>();
            manageUnitSystem?.Units.Add(spawned.gameObject);
        }

        private void CreatePlayerTeamLeaderFromPool()
        {
            var pool = PoolKit.Find("Team Leader Pool");
            if (pool == null) return;
            // TODO: Should create from factory and give location later
            var randomPosition = new Vector3(
                Random.Range(0, 100.0f),
                0,
                Random.Range(0, 100.0f));

            var spawned = pool.Spawn("Character_Female_Jacket_01", randomPosition, Quaternion.identity);

            var decisionController = spawned.GetComponent<DecisionController>();
            if (decisionController != null)
            {
                decisionController.enabled = false;
            }

            var behaviorTree = spawned.GetComponent<BehaviorTree>();
            if (behaviorTree != null)
            {
                behaviorTree.DisableBehavior();
                behaviorTree.enabled = false;
            }

            // var aiLerp = spawned.GetComponent<AILerp>();
            // if (aiLerp != null)
            // {
            //     aiLerp.enabled = false;
            // }

            var controllingRoute = spawned.GetComponent<GameInputControl.ControllingRoute>();
            if (controllingRoute != null)
            {
                controllingRoute.autoGenerate = false;
            }

            // // Setup input
            // spawned.gameObject.AddComponent<GameInputControl.PlayerInputControl>();
            // //
            // spawned.gameObject.AddComponent<GameInputControl.FormRouteFromInput>();
            
            // Setup camera to follow
#if THIRD_PERSON_CONTROLLER

            // spawned.gameObject.AddComponent<UnityInput>();

            var createdPlayerCamera = Instantiate(quickPlayerCamera);

            var scene = SceneManager.GetSceneByName("Entry");
            SceneManager.MoveGameObjectToScene(createdPlayerCamera, scene);
            
            var cameraController = createdPlayerCamera.GetComponent<CameraController>();
            if (cameraController != null)
            {
                cameraController.Character = spawned.gameObject;
                
                // cameraController.gameObject.SetActive(true);
                
                // var viewTypeName = "Opsive.UltimateCharacterController.ThirdPersonController.Camera.ViewTypes.TopDown";
                // cameraController.SetViewType(UnityEngineUtility.GetType(viewTypeName), false);
            }
            spawned.SetParent(null);

            // var cameraControllerHandler = FindObjectOfType<CameraControllerHandler>();
            // if (cameraControllerHandler != null)
            // {
            //     cameraControllerHandler.enabled = false;
            // }

#endif
        }
        
        public void OnSpawn(Pool pool)
        {
            _logger.Debug($"Bootstrap - OnSpawn - pool: {pool}");
        }

        public void OnDespawn()
        {
            _logger.Debug($"Bootstrap - OnDespawn");
        }
    }
}
